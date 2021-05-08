using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UltEvents;
using UnityEngine;

namespace Level {
    [DisallowMultipleComponent]
    public class LevelGenerator : MonoBehaviour {
        private GridXY<Element> newLevel;

        [Header("Settings")]
        private const int DEFAULT_WIDTH = 5;
        private const int DEFAULT_HEIGHT = 5;
        private const int DEFAULT_NODES_PERCENTAGE = 1;
        [SerializeField, Disable] private int lvlWidth;
        [SerializeField, Disable] private int lvlHeight;
        [SerializeField, Disable] private int nodesPercentage;
        System.Random rnd;

        private Thread generating;
        private enum LevelGenerationStatus {
            Idle,
            Generating,
            Completed,
            Abort
        }
        private LevelGenerationStatus status;
        public bool stopGenerating = false;
        private float genProgressionPrev;
        [SerializeField, ProgressBar("Generation Progress", minValue: 0f, maxValue: 1f, HexColor = "#EB7D34")] private float genProgression;

        [Header("Events")]
        [SerializeField] private UltEvent GenerationStarted;
        [SerializeField] private UltEvent GenerationStopped;
        [SerializeField] private UltEvent GenerationAborted;
        [SerializeField] private UltEvent GenerationCompleted;
        [SerializeField] private UltEventFloat GenerationProgress;

        private void Awake() {
            ResetSettings();
        }

        private void Update() {
            if(status == LevelGenerationStatus.Abort) {
                AbortGeneration();
                GenerationAborted?.Invoke();
            }

            if(genProgressionPrev != genProgression) {
                GenerationProgress?.Invoke(genProgression);
                genProgressionPrev = genProgression;
            }

            if (status == LevelGenerationStatus.Completed) {
                Debug.Log("Generated level");
                status = LevelGenerationStatus.Idle;
                GenerationStopped?.Invoke();
                if (newLevel != null) {
                    LevelManager.Main.LoadLevel(newLevel);
                } else {
                    Debug.LogWarning("Level generated is not valid, abort");
                }
                GenerationCompleted?.Invoke();
            }
        }

        public void ResetSettings() {
            lvlWidth = DEFAULT_WIDTH;
            lvlHeight = DEFAULT_HEIGHT;
            nodesPercentage = DEFAULT_NODES_PERCENTAGE;
        }

        public void SetWidth(float value) { lvlWidth = Mathf.RoundToInt(value); }
        public void SetHeight(float value) { lvlHeight = Mathf.RoundToInt(value); }
        public void SetNodesPercentage(float value) { nodesPercentage = Mathf.RoundToInt(value); }

        public void ApplySettings() {
            Debug.Log("Before start thread");
            generating = new Thread(GenerateLevel);
            generating.Start();
            GenerationStarted?.Invoke();
            Debug.Log("Started thread");
        }

        private void GenerateLevel() {
            status = LevelGenerationStatus.Idle;
            genProgressionPrev = -1;
            genProgression = 0;
            rnd = new System.Random(System.DateTime.Now.Millisecond);

            if (lvlWidth <= 0) {
                Debug.LogError($"Can't generate a level with {lvlWidth} width");
                status = LevelGenerationStatus.Abort;
            }

            if (lvlHeight <= 0) {
                Debug.LogError($"Can't generate a level with {lvlHeight} height");
                status = LevelGenerationStatus.Abort;
            }

            Debug.Log($"Generating level {lvlWidth}x{lvlHeight}");

            newLevel = new GridXY<Element>();
            newLevel.CreateGridXY(lvlWidth, lvlHeight, 1, Vector3.zero, false, Element.NULL, Element.NULL);
            LevelNavigation.SetUp(lvlWidth, lvlHeight, true, false);

            List<Vector2Int> nodes = GenerateNodes(Mathf.RoundToInt(Mathf.Clamp((lvlWidth * lvlHeight) * (nodesPercentage / 100f), 2, newLevel.Size)));
            int paths = GeneratePaths(nodes);
            if (paths > 0) {
                status = LevelGenerationStatus.Completed;
            } else {
                status = LevelGenerationStatus.Abort;
            }
        }

        private List<Vector2Int> GenerateNodes(int num) {
            if (num > 0) {
                int attempts = 0;
                int maxAttempts = num * 2;

                List<Vector2Int> nodesAlreadyFound = new List<Vector2Int>();
                Vector2Int cell = Vector2Int.zero;
                System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
                for (int i = 0; i < num && attempts < maxAttempts; i++, attempts++) {
                    cell = new Vector2Int(rnd.Next(0, newLevel.Width), rnd.Next(0, newLevel.Height));
                    if (newLevel.GetTile(cell).IsNodeType() || nodesAlreadyFound.Contains(cell)) {
                        i--;
                        continue;
                    }
                    nodesAlreadyFound.Add(cell);
                    newLevel.SetTile(cell, Element.Node);
                }
                Debug.Log($"Generated {nodesAlreadyFound.Count}/{num} nodes in {attempts}/{maxAttempts} attempts");

                return nodesAlreadyFound;
            }

            Debug.LogWarning($"Can't generate {num} nodes");
            return null;
        }

        private int GeneratePaths(List<Vector2Int> nodes) {
            if (nodes.Count > 0) {
                int attempts = 0;
                int maxAttempts = nodes.Count * 2;

                List<LevelNavigation.PathNode> newPath = new List<LevelNavigation.PathNode>();
                List<bool> nodesUsed = Enumerable.Repeat(false, nodes.Count).ToList();

                int path = 0;
                int nodesUnusable = 0;
                for (path = 0; path + nodesUnusable + 1 < nodes.Count && attempts < maxAttempts; path++, attempts++) {
                    newPath = LevelNavigation.FindPath(nodes[path], nodes[path + nodesUnusable + 1], newLevel);
                    if (newPath == null) {
                        path--;
                        nodesUnusable += (path + nodesUnusable < nodes.Count - 2) ? 1 : 0;
                    } else {
                        ApplyPath(newPath);
                        foreach (LevelNavigation.PathNode p in newPath.Where(tmpP => nodes.Contains(tmpP.GetCell())).ToList()) {
                            nodesUsed[nodes.IndexOf(p.GetCell())] = true;
                        }
                        path += nodesUnusable;
                        nodesUnusable = 0;
                    }
                    genProgression = Mathf.Max((float)path / nodes.Count, (float)attempts / maxAttempts);
                }

                Debug.Log($"Generated {path}/{nodes.Count - 1} paths in {attempts}/{maxAttempts} attempts");

                //SearchForNodeClusters(nodes, nodesUsed);

                nodes = RemoveUnusedNodes(nodes, nodesUsed);

                newLevel.SetTile(nodes[0], Element.Start);
                newLevel.SetTile(nodes[nodes.Count - 1], Element.End);

                return nodes.Count;
            }

            Debug.LogWarning("There are no nodes for which generate paths");
            return 0;
        }

        private void SearchForNodeClusters(List<Vector2Int> nodes, List<bool> nodesUsed) {
            for (int i = 0; i < nodesUsed.Count; i++) {
                if (nodesUsed[i]) {
                    List<Vector2Int> neighbours = newLevel.GatherNeighbourCells(nodes[i], 1, true, true);
                    neighbours = neighbours.Where(n => nodes.Contains(n)).ToList();
                    if (neighbours.Count > 0) {
                        foreach (Vector2Int neighbour in neighbours) {
                            nodesUsed[nodes.IndexOf(neighbour)] = true;
                        }
                    }
                }
            }
        }

        private List<Vector2Int> RemoveUnusedNodes(List<Vector2Int> nodes, List<bool> nodesUsed) {
            List<Vector2Int> result = new List<Vector2Int>();
            for (int i = 0; i < nodes.Count; i++) {
                if (!nodesUsed[i]) {
                    newLevel.SetTile(nodes[i], Element.NULL);
                } else {
                    result.Add(nodes[i]);
                }
            }
            Debug.Log($"Removed unused nodes");
            return result;
        }

        private void ApplyPath(List<LevelNavigation.PathNode> path) {
            if (path != null) {
                if (!newLevel.GetTile(path[0].GetCell()).IsNodeType()) {
                    newLevel.SetTile(path[0].GetCell(), Element.Node);
                }
                if (!newLevel.GetTile(path.Last().GetCell()).IsNodeType()) {
                    newLevel.SetTile(path.Last().GetCell(), Element.Node);
                }
                Element newTile;
                for (int i = 1; i < path.Count - 1; i++) {
                    if (newLevel.GetTile(path[i].GetCell()).IsNodeType()) {
                        continue;
                    }
                    newTile = Element.Node;
                    if (path[i].X < path[i + 1].X && path[i].Y == path[i + 1].Y)
                        newTile = Element.Right;
                    if (path[i].X > path[i + 1].X && path[i].Y == path[i + 1].Y)
                        newTile = Element.Left;
                    if (path[i].X == path[i + 1].X && path[i].Y > path[i + 1].Y)
                        newTile = Element.Down;
                    if (path[i].X == path[i + 1].X && path[i].Y < path[i + 1].Y)
                        newTile = Element.Up;
                    newLevel.SetTile(path[i].GetCell(), newTile);
                }
            }
        }

        public void AbortGeneration() {
            if (generating != null) {
                Debug.Log("Aborting Thread");
                status = LevelGenerationStatus.Idle;
                generating.Abort();
                GenerationStopped?.Invoke();
            }
        }
    }
}