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
        private float genProgression;

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
                    LevelManager.main.LoadLevel(newLevel.ToSeedString());
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
            Pathfinding pathfinding = new Pathfinding(lvlWidth, lvlHeight, true, false);

            List<Vector2Int> nodes = GenerateNodes(Mathf.RoundToInt(Mathf.Clamp((lvlWidth * lvlHeight) * (nodesPercentage / 100f), 2, newLevel.Size)));
            int paths = GeneratePaths(nodes, pathfinding);
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

        private int GeneratePaths(List<Vector2Int> nodes, Pathfinding pathfinding) {
            if (nodes.Count > 0) {
                int attempts = 0;
                int maxAttempts = nodes.Count * 2;

                List<Pathfinding.PathNode> newPath = new List<Pathfinding.PathNode>();
                List<bool> nodesUsed = Enumerable.Repeat(false, nodes.Count).ToList();

                int path;
                int nodesUnusable = 0;
                for (path = 0; path + nodesUnusable + 1 < nodes.Count - 1 && attempts < maxAttempts; path++, attempts++) {
                    newPath = pathfinding.FindPath(nodes[path], nodes[path + nodesUnusable + 1], newLevel);
                    if (newPath == null) {
                        path--;
                        nodesUnusable += (path + nodesUnusable < nodes.Count - 2) ? 1 : 0;
                    } else {
                        ApplyPath(newPath);
                        foreach (Pathfinding.PathNode p in newPath.Where(tmpP => nodes.Contains(tmpP.GetCell())).ToList()) {
                            nodesUsed[nodes.IndexOf(p.GetCell())] = true;
                        }
                        path += nodesUnusable;
                        nodesUnusable = 0;
                    }
                    genProgression = Mathf.Max((float)path / nodes.Count, (float)attempts / maxAttempts);
                }

                Debug.Log($"Generated {path}/{nodes.Count} paths in {attempts}/{maxAttempts} attempts");

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

        private void ApplyPath(List<Pathfinding.PathNode> path) {
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


    public class Pathfinding {
        private const int MOVE_STRAIGHT_COST = 1;

        public class PathNode {
            private GridXY<PathNode> grid;
            public int X { get; private set; }
            public int Y { get; private set; }

            public int gCost;
            public int hCost;
            public int FCost { get { return gCost + hCost; } }

            public PathNode previousNode;

            public PathNode(GridXY<PathNode> grid, int x, int y) {
                this.grid = grid;
                X = x;
                Y = y;
            }

            public Vector2Int GetCell() {
                return new Vector2Int(X, Y);
            }

            public override string ToString() {
                return $"x{X}, y{Y}";
            }
        }

        private GridXY<PathNode> grid;
        private GridXY<Element> gridElements;
        private List<PathNode> openList;
        private List<PathNode> closedList;
        private bool includeNullElements;
        private bool searchBestPath;

        public Pathfinding(int width, int height, bool includeNullElements, bool searchBestPath) {
            grid = new GridXY<PathNode>();
            grid.CreateGridXY(width, height, 1, Vector3.zero, true, null, (GridXY<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
            this.includeNullElements = includeNullElements;
            this.searchBestPath = searchBestPath;
        }

        public List<PathNode> FindPath(Vector2Int startCell, Vector2Int endCell, GridXY<Element> gridElements) {
            this.gridElements = gridElements;

            PathNode startNode = grid.GetTile(startCell.x, startCell.y);
            PathNode endNode = grid.GetTile(endCell.x, endCell.y);

            if(startNode == null) {
                Debug.LogError($"Can't find path because start node on cell {startCell} is null");
                return null;
            }
            if (endNode == null) {
                Debug.LogError($"Can't find path because end node on cell {endCell} is null");
                return null;
            }

            openList = new List<PathNode>() { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    PathNode pathNode = grid.GetTile(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.previousNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);

            while (openList.Count > 0) {
                PathNode currentNode;

                if (searchBestPath) {
                    currentNode = GetLowestFCostNode(openList);
                } else {
                    currentNode = openList[new System.Random().Next(0, openList.Count - 1)];
                }

                if (currentNode == endNode) {
                    // Reached final node
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                    if (closedList.Contains(neighbourNode)) {
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost) {
                        neighbourNode.previousNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                        if (!openList.Contains(neighbourNode)) {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            // No Path
            return null;
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode) {
            List<PathNode> neighbourList = new List<PathNode>();
            Direction currentNodeDirection = gridElements.GetTile(currentNode.GetCell()).ToDirection();

            if (currentNodeDirection != Direction.NULL && currentNodeDirection != Direction.All) {
                Vector2Int cellFaced = currentNode.GetCell() + currentNodeDirection.ToOffset();
                if (grid.CellIsValid(cellFaced)) {
                    neighbourList.Add(grid.GetTile(cellFaced));
                }
            } else {
                //Left
                if (grid.CellIsValid(currentNode.X - 1, currentNode.Y)) {
                    neighbourList.Add(grid.GetTile(currentNode.X - 1, currentNode.Y));
                }

                // Right
                if (grid.CellIsValid(currentNode.X + 1, currentNode.Y)) {
                    neighbourList.Add(grid.GetTile(currentNode.X + 1, currentNode.Y));
                }

                // Up
                if (grid.CellIsValid(currentNode.X, currentNode.Y - 1)) {
                    neighbourList.Add(grid.GetTile(currentNode.X, currentNode.Y - 1));
                }

                // Down
                if (grid.CellIsValid(currentNode.X, currentNode.Y + 1)) {
                    neighbourList.Add(grid.GetTile(currentNode.X, currentNode.Y + 1));
                }
            }

            if (!includeNullElements) {
                neighbourList = neighbourList.Where(p => gridElements.GetTile(p.GetCell()) != Element.NULL).ToList();
            }

            return neighbourList;
        }

        private List<PathNode> CalculatePath(PathNode endNode) {
            List<PathNode> path = new List<PathNode>() { endNode };
            PathNode currentNode = endNode;
            while(currentNode.previousNode != null) {
                path.Add(currentNode.previousNode);
                currentNode = currentNode.previousNode;
            }
            path.Reverse();
            return path;
        }

        private int CalculateDistanceCost(PathNode a, PathNode b) {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            return MOVE_STRAIGHT_COST * (xDistance + yDistance);
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                    lowestFCostNode = pathNodeList[i];
            return lowestFCostNode;
        }
    }
}