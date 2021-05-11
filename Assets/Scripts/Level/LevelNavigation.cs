using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Custom A* algorythm used to navigate a level and also generate one.
/// </summary>
public static class LevelNavigation {
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

    private static GridXY<PathNode> grid;
    private static GridXY<Element> gridElements;
    private static List<PathNode> openList;
    private static List<PathNode> closedList;
    private static bool includeNullElements;
    private static bool searchBestPath;

    public static bool IsReady {
        get {
            return
              grid != null &&
              grid.Size > 0 &&
              gridElements != null &&
              gridElements.Size > 0;
        }
    }

    public static void SetUp(int width, int height, bool includeNullElements, bool searchBestPath) {
        grid = new GridXY<PathNode>();
        grid.CreateGridXY(width, height, 1, Vector3.zero, true, null, (GridXY<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
        LevelNavigation.includeNullElements = includeNullElements;
        LevelNavigation.searchBestPath = searchBestPath;
    }
    public static void SetUp(int width, int height, bool includeNullElements, bool searchBestPath, GridXY<Element> gridElements) {
        LevelNavigation.gridElements = gridElements;
        SetUp(width, height, includeNullElements, searchBestPath);
    }

    public static List<PathNode> FindPath(Vector2Int startCell, Vector2Int endCell) {
        PathNode startNode = grid.GetTile(startCell.x, startCell.y);
        PathNode endNode = grid.GetTile(endCell.x, endCell.y);

        if (startNode == null) {
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
    public static List<PathNode> FindPath(Vector2Int startCell, Vector2Int endCell, GridXY<Element> gridElements) {
        LevelNavigation.gridElements = gridElements;
        return FindPath(startCell, endCell);
    }

    private static List<PathNode> GetNeighbourList(PathNode currentNode) {
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

    private static List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>() { endNode };
        PathNode currentNode = endNode;
        while (currentNode.previousNode != null) {
            path.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }
        path.Reverse();
        return path;
    }

    private static int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.X - b.X);
        int yDistance = Mathf.Abs(a.Y - b.Y);
        return MOVE_STRAIGHT_COST * (xDistance + yDistance);
    }

    private static PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
            if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                lowestFCostNode = pathNodeList[i];
        return lowestFCostNode;
    }
}