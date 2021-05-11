using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private enum LevelGenerationStatus {
            Idle,
            Generating,
            Nodes,
            Paths,
            Completed,
            Abort
        }
        private LevelGenerationStatus status;
        private LevelGenerationStatus Status {
            get { return status; }
            set {
                LevelGenerationStatus oldStatus = status;
                status = value;
                if (oldStatus != value) {
                    GenerationStatusChanged?.Invoke(GetStatusText(value));
                }
            }
        }
        public bool stopGenerating = false;
        private float genProgressionPrev;
        [SerializeField, ProgressBar("Generation Progress", minValue: 0f, maxValue: 1f, HexColor = "#EB7D34")] private float genProgression;

        [Header("Events")]
        [SerializeField] private UltEvent GenerationStarted;
        [SerializeField] private UltEvent GenerationStopped;
        [SerializeField] private UltEvent GenerationAborted;
        [SerializeField] private UltEvent GenerationCompleted;
        [SerializeField] private UltEventFloat GenerationProgress;
        [SerializeField] private UltEventString GenerationStatusChanged;

        private void Awake() {
            ResetSettings();
        }

        private void Update() {
            if(Status == LevelGenerationStatus.Abort) {
                AbortGeneration();
                GenerationAborted?.Invoke();
            }

            if(genProgressionPrev != genProgression) {
                GenerationProgress?.Invoke(genProgression);
                genProgressionPrev = genProgression;
            }

            if (Status == LevelGenerationStatus.Completed) {
                Debug.Log("Generated level");
                Status = LevelGenerationStatus.Idle;
                GenerationStopped?.Invoke();

                if(newLevel == null) {
                    Debug.LogWarning("Level generated is not valid, abort");
                    AbortGeneration();
                    return;
                }

                LevelManager.Main.LoadLevel(newLevel);
                GenerationCompleted?.Invoke();
            }
        }

        public void ResetSettings() {
            lvlWidth = DEFAULT_WIDTH;
            lvlHeight = DEFAULT_HEIGHT;
            nodesPercentage = DEFAULT_NODES_PERCENTAGE;
        }

        /// <summary>
        /// Set level generation width.
        /// </summary>
        /// <param name="value">Level width.</param>
        public void SetWidth(float value) { lvlWidth = Mathf.RoundToInt(value); }

        /// <summary>
        /// Set level generation height.
        /// </summary>
        /// <param name="value">Level height.</param>
        public void SetHeight(float value) { lvlHeight = Mathf.RoundToInt(value); }

        /// <summary>
        /// Set level generation nodes' percentage.
        /// </summary>
        /// <param name="value">Level nodes' percentage.</param>
        public void SetNodesPercentage(float value) { nodesPercentage = Mathf.RoundToInt(value); }

        /// <summary>
        /// Generate new level.
        /// </summary>
        public void ApplySettings() {
            Debug.Log("Before start thread");
            StartCoroutine(GenerateLevel());
            GenerationStarted?.Invoke();
            Debug.Log("Started thread");
        }

        private IEnumerator GenerateLevel() {

            if (lvlWidth <= 0) {
                Debug.LogError($"Can't generate a level with {lvlWidth} width");
                Status = LevelGenerationStatus.Abort;
                yield break;
            }

            if (lvlHeight <= 0) {
                Debug.LogError($"Can't generate a level with {lvlHeight} height");
                Status = LevelGenerationStatus.Abort;
                yield break;
            }

            Status = LevelGenerationStatus.Idle;
            genProgressionPrev = -1;
            genProgression = 0;
            rnd = new System.Random(DateTime.Now.Millisecond);

            Debug.Log($"Generating level {lvlWidth}x{lvlHeight}");

            newLevel = new GridXY<Element>();
            newLevel.CreateGridXY(lvlWidth, lvlHeight, 1, Vector3.zero, false, Element.NULL, Element.NULL);
            LevelNavigation.SetUp(lvlWidth, lvlHeight, true, false);

            Status = LevelGenerationStatus.Generating;
            yield return null;

            List<Vector2Int> nodes = new List<Vector2Int>();

            Coroutine generatingNodes = StartCoroutine(GenerateNodes(Mathf.RoundToInt(Mathf.Clamp((lvlWidth * lvlHeight) * (nodesPercentage / 100f), 2, newLevel.Size)), (generatedNodes) => { nodes = generatedNodes; }));
            yield return generatingNodes;

            yield return null;

            int pathsNum = 0;
            Coroutine generatingPaths = StartCoroutine(GeneratePaths(nodes, (generatedPathsNum) => { pathsNum = generatedPathsNum; }));
            yield return generatingPaths;

            yield return null;

            if (pathsNum > 0) {
                Status = LevelGenerationStatus.Completed;
            } else {
                Status = LevelGenerationStatus.Abort;
            }
        }

        private IEnumerator GenerateNodes(int num, Action<List<Vector2Int>> callback) {

            if(num <= 0) {
                Debug.LogWarning($"Can't generate {num} nodes");
                yield break;
            }

            int attempts = 0;
            int maxAttempts = num * 2;

            List<Vector2Int> nodesAlreadyFound = new List<Vector2Int>();
            Vector2Int cell = Vector2Int.zero;
            System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);

            Status = LevelGenerationStatus.Nodes;
            yield return null;

            for (int i = 0; i < num && attempts < maxAttempts; i++, attempts++) {
                cell = new Vector2Int(rnd.Next(0, newLevel.Width), rnd.Next(0, newLevel.Height));
                if (newLevel.GetTile(cell).IsNodeType() || nodesAlreadyFound.Contains(cell)) {
                    i--;
                    continue;
                }
                nodesAlreadyFound.Add(cell);
                newLevel.SetTile(cell, Element.Node);
                genProgression = Mathf.Max((float)i / num, (float)attempts / maxAttempts);
                yield return null;
            }
            Debug.Log($"Generated {nodesAlreadyFound.Count}/{num} nodes in {attempts}/{maxAttempts} attempts");

            callback?.Invoke(nodesAlreadyFound);
            yield break;
        }

        private IEnumerator GeneratePaths(List<Vector2Int> nodes, Action<int> callback) {
            if(nodes.Count <= 0) {
                Debug.LogWarning("There are no nodes for which generate paths");
                yield break;
            }

            int attempts = 0;
            int maxAttempts = nodes.Count * 2;

            List<LevelNavigation.PathNode> newPath = new List<LevelNavigation.PathNode>();
            List<bool> nodesUsed = Enumerable.Repeat(false, nodes.Count).ToList();

            int path = 0;
            int nodesUnusable = 0;

            Status = LevelGenerationStatus.Paths;
            yield return null;

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
                yield return null;
            }

            Debug.Log($"Generated {path}/{nodes.Count - 1} paths in {attempts}/{maxAttempts} attempts");

            nodes = RemoveUnusedNodes(nodes, nodesUsed);

            newLevel.SetTile(nodes[0], Element.Start);
            newLevel.SetTile(nodes[nodes.Count - 1], Element.End);

            callback?.Invoke(nodes.Count);
            yield break;
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
            if(path == null) {
                return;
            }

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

        /// <summary>
        /// Abort level generation.
        /// </summary>
        public void AbortGeneration() {
            if (!IsGenerating()) {
                return;
            }

            Debug.Log("Aborting Thread");
            Status = LevelGenerationStatus.Idle;
            StopAllCoroutines();
            GenerationStopped?.Invoke();
        }

        private bool IsGenerating() {
            return
                Status != LevelGenerationStatus.Idle &&
                Status != LevelGenerationStatus.Completed &&
                Status != LevelGenerationStatus.Abort;
        }

        private string GetStatusText(LevelGenerationStatus status) {
            string result = "";

            switch (status) {
                case LevelGenerationStatus.Generating: result = "Generating..."; break;
                case LevelGenerationStatus.Nodes: result = "Generating Nodes..."; break;
                case LevelGenerationStatus.Paths: result = "Generating Paths..."; break;
                case LevelGenerationStatus.Completed: result = "Complete!"; break;
            }

            return result;
        }
    }
}