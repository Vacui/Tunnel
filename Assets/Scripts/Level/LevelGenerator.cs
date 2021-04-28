using System.Collections.Generic;
using UnityEngine;

namespace Level {
    public static class LevelGenerator {

        private const int WIDTH_MIN = 1;
        private const int WIDTH_MAX = 20;
        private const int HEIGHT_MIN = 1;
        private const int HEIGHT_MAX = 20;

        public static void GenerateLevel(int width, int height) {
            width = Mathf.Clamp(width, WIDTH_MIN, WIDTH_MAX);
            height = Mathf.Clamp(height, HEIGHT_MIN, HEIGHT_MAX);

            Debug.Log($"Generating level {width}x{height}");

            //string newLevelSeed = $"{width}/{height}/1";
            //for (int i = 1; i < (width * height) - 1; i++)
            //    newLevelSeed += "-3";
            //newLevelSeed += "-2";

            //Debug.Log($"Generated seed: {newLevelSeed}");

            //LevelManager.main.LoadLevel(newLevelSeed);


            Pathfinding pathfinding = new Pathfinding(width, height);
            List<Pathfinding.PathNode> path = pathfinding.FindPath(new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1));
            Debug.Log(path.Count);

            GridXY<Element> newLevel = new GridXY<Element>();
            newLevel.CreateGridXY(width, height, 1, Vector3.zero, false, Element.NULL, Element.NULL);
            newLevel.SetTile(path[0].GetCell(), Element.Start);
            Debug.Log($"Start: {path[0]}");
            newLevel.SetTile(path.Last().GetCell(), Element.End);
            Debug.Log($"End: {path.Last()}");
            for (int i = 1; i < path.Count - 1; i++) {
                newLevel.SetTile(path[i].GetCell(), Element.Node);
                Debug.Log($"Node: {path[i]}");
            }
            LevelManager.main.LoadLevel(newLevel.ToSeedString());
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
        private List<PathNode> openList;
        private List<PathNode> closedList;

        public Pathfinding(int width, int height) {
            grid = new GridXY<PathNode>();
            grid.CreateGridXY(width, height, 1, Vector3.zero, true, null, (GridXY<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
        }

        public List<PathNode> FindPath(Vector2Int startCell, Vector2Int endCell) {
            PathNode startNode = grid.GetTile(startCell.x, startCell.y);
            PathNode endNode = grid.GetTile(endCell.x, endCell.y);

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
                PathNode currentNode = GetLowestFCostNode(openList);

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

            // Left
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