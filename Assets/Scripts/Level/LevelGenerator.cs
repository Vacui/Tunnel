using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level {
    public static class LevelGenerator {
        private static GridXY<Element> newLevel;

        public static GridXY<Element> GenerateLevel(int width, int height, int nodesPercentage) {

            if(width <= 0) {
                GameDebug.LogError($"Can't generate a level with {width} width");
                return null;
            }

            if (height <= 0) {
                GameDebug.LogError($"Can't generate a level with {height} height");
                return null;
            }

            GameDebug.Log($"Generating level {width}x{height}");

            newLevel = new GridXY<Element>();
            newLevel.CreateGridXY(width, height, 1, Vector3.zero, false, Element.NULL, Element.NULL);
            Pathfinding pathfinding = new Pathfinding(width, height, true, false);

            List<Vector2Int> nodes = GenerateNodes(Mathf.RoundToInt(Mathf.Clamp((width * height) * (nodesPercentage / 100f), 2, newLevel.Size)));
            int paths = GeneratePaths(nodes, pathfinding);
            if (paths < nodes.Count / 2) {
                return newLevel;
            } else {
                GameDebug.LogWarning($"Generated just {paths} paths");
                return null;
            }
        }

        private static List<Vector2Int> GenerateNodes(int num) {
            if (num > 0) {
                int attempts = 0;
                int maxAttempts = num * 2;

                List<Vector2Int> nodesAlreadyFound = new List<Vector2Int>();
                Vector2Int cell = Vector2Int.zero;
                for (int i = 0; i < num && attempts < maxAttempts; i++, attempts++) {
                    cell = new Vector2Int(Random.Range(0, newLevel.Width), Random.Range(0, newLevel.Height));
                    if (IsNodeType(newLevel.GetTile(cell)) || nodesAlreadyFound.Contains(cell)) {
                        i--;
                        continue;
                    }
                    nodesAlreadyFound.Add(cell);
                    newLevel.SetTile(cell, Element.Node);
                }
                GameDebug.Log($"Generated {nodesAlreadyFound.Count}/{num} nodes, attempts={attempts}/{maxAttempts}");

                return nodesAlreadyFound;
            }

            GameDebug.LogWarning($"Can't generate {num} nodes");
            return null;
        }

        private static int GeneratePaths(List<Vector2Int> nodes, Pathfinding pathfinding) {
            if (nodes.Count > 0) {
                int attempts = 0;
                int maxAttempts = nodes.Count * 2;

                List<Pathfinding.PathNode> newPath = new List<Pathfinding.PathNode>();
                List<bool> usedNodes = Enumerable.Repeat(false, nodes.Count).ToList();

                int path;
                for (path = 0; path < nodes.Count - 1 && attempts < nodes.Count * 2; path++, attempts++) {
                    newPath = pathfinding.FindPath(nodes[path], nodes[path + 1], newLevel);
                    if (newPath == null) {
                        path--;
                        continue;
                    }
                    ApplyPath(newPath);
                    foreach (Pathfinding.PathNode p in newPath.Where(tmpP => nodes.Contains(tmpP.GetCell())).ToList()) {
                        usedNodes[nodes.IndexOf(p.GetCell())] = true;
                    }
                }

                for(int i = 0; i < usedNodes.Count; i++) {
                    if (usedNodes[i]) {
                        List<Vector2Int> neighbours = newLevel.GatherNeighbourCells(nodes[i]);
                        neighbours = neighbours.Where(n => nodes.Contains(n)).ToList();
                        if (neighbours.Count > 0) {
                            foreach(Vector2Int neighbour in neighbours) {
                                usedNodes[nodes.IndexOf(neighbour)] = true;
                            }
                        }
                    }
                }

                int unusedNodesCount = usedNodes.Where(n => n == false).Count();
                GameDebug.Log($"Connected {nodes.Count - unusedNodesCount}/{nodes.Count} nodes, attempts={attempts}/{maxAttempts}");


                for (int i = 0; i < nodes.Count; i++) {
                    if (!usedNodes[i]) {
                        newLevel.SetTile(nodes[i], Element.NULL);
                    }
                }
                GameDebug.Log($"Deleted {unusedNodesCount} unused nodes");

                newLevel.SetTile(nodes[0], Element.Start);
                newLevel.SetTile(nodes[Random.Range(1, nodes.Count - unusedNodesCount)], Element.End);

                return unusedNodesCount;
            }

            GameDebug.LogWarning("There are no nodes for which generate paths");
            return 0;
        }

        private static void ApplyPath(List<Pathfinding.PathNode> path) {
            if (path != null) {
                if (!IsNodeType(newLevel.GetTile(path[0].GetCell()))) {
                    newLevel.SetTile(path[0].GetCell(), Element.Node);
                }
                if (!IsNodeType(newLevel.GetTile(path.Last().GetCell()))) {
                    newLevel.SetTile(path.Last().GetCell(), Element.Node);
                }
                Element newTile;
                for (int i = 1; i < path.Count - 1; i++) {
                    if (IsNodeType(newLevel.GetTile(path[i].GetCell()))) {
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
        private static bool IsNodeType(Element element) {
            return element.ToDirection() == Direction.All;
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
                GameDebug.LogError($"Can't find path because start node on cell {startCell} is null");
                return null;
            }
            if (endNode == null) {
                GameDebug.LogError($"Can't find path because end node on cell {endCell} is null");
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
                    currentNode = openList[Random.Range(0, openList.Count - 1)];
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