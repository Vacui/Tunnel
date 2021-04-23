using PlayerLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    [DisallowMultipleComponent, RequireComponent(typeof(Tilemap))]
    public class LevelFog : MonoBehaviour
    {
        public static LevelFog main { get; private set; }

        private GridXY<TileVisibility> grid;

        [Header("Visuals")]
        private Tilemap tilemap;
        [SerializeField] private TileBase visual;

        [Header("Stats")]
        [SerializeField, Disable] private int tilesTotal;
        [SerializeField, Disable] private int tilesHidden;
        public float LevelExplorationPercentage { get { return (tilesTotal - tilesHidden) / (float)tilesTotal; } }

        public event EventHandler<DiscoveredTileEventArgs> DiscoveredTile;
        public class DiscoveredTileEventArgs : EventArgs
        {
            public int x, y;
            public float levelExplorationPercentage;
        }

        [SerializeField] private bool showDebugLog = false;

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);

            tilemap = GetComponent<Tilemap>();
            grid = new GridXY<TileVisibility>();

            LevelManager.main.grid.OnGridCreated += (sender, args) =>
            {
                tilesTotal = args.width * args.height;
                tilesHidden = tilesTotal;
                tilemap.ClearAllTiles();
                grid.CreateGridXY(args.width, args.height, args.cellSize, args.originPosition, TileVisibility.Invisible);
            };
            LevelManager.main.grid.OnTileChanged += (sender, args) =>
            {
                HideTile(args.x, args.y);
                if (args.value == TileType.Goal)
                    DiscoverTile(args.x, args.y);
            };

            grid.OnTileChanged += (sender, args) =>
            {
                tilemap.SetTile(new Vector3Int(args.x, args.y, 0), args.value == TileVisibility.Invisible ? visual : null);
                CheckTilesVisibilityAround(args.x, args.y);
            };

            LevelManager.OnLevelReady += (sender, args) => CheckNullTiles();

            Player.StartedMove += (sender, args) => DiscoverTile(args.x, args.y);
            Player.Moved += (sender, args) => DiscoverTile(args.x, args.y);
            Player.StoppedMove += (sender, args) =>
            {
                DiscoverTile(args.x + 1, args.y);
                DiscoverTile(args.x - 1, args.y);
                DiscoverTile(args.x, args.y + 1);
                DiscoverTile(args.x, args.y - 1);
            };
        }

        private void SetTileVisibility(int x, int y, TileVisibility visibility)
        {
            if (grid.CellIsValid(x, y) && (visibility == TileVisibility.Invisible || grid.GetTile(x, y) != visibility))
            {
                if (visibility == TileVisibility.Visible)
                {
                    tilesHidden--;
                    DiscoveredTile?.Invoke(this, new DiscoveredTileEventArgs { x = x, y = y, levelExplorationPercentage = LevelExplorationPercentage });
                }
                if (showDebugLog) Debug.Log($"Setting Tile {x},{y} Visibility ({visibility})");
                grid.SetTile(x, y, visibility);
            }
        }
        private void DiscoverTile(int x, int y) { SetTileVisibility(x, y, TileVisibility.Visible); }
        private void HideTile(int x, int y) { SetTileVisibility(x, y, TileVisibility.Invisible); }

        private void CheckNullTiles()
        {
            for (int x = 0; x < grid.width; x++)
                for (int y = 0; y < grid.height; y++)
                    IsNullTileReadyToVisible(x, y);
        }

        private bool IsNullTileReadyToVisible(int x, int y)
        {
            bool isReadyToVisible = false;

            if (grid.CellIsValid(x, y))
            {
                if (LevelManager.main.grid.GetTile(x, y) == TileType.NULL && grid.GetTile(x, y) == TileVisibility.Invisible)
                {
                    if (showDebugLog) Debug.Log($"Is Tile {x},{y} ReadyToVisible?");
                    isReadyToVisible = true;

                    TileType type;
                    TileVisibility visibility;

                    List<Vector2Int> neighbours = MyUtils.GatherNeighbours(x, y, 1, true, false);
                    Vector2Int neighbour;
                    for (int i = 0; i < neighbours.Count && isReadyToVisible; i++)
                    {
                        neighbour = neighbours[i];
                        if (LevelManager.main.grid.CellIsValid(neighbour.x, neighbour.y) && grid.CellIsValid(neighbour.x, neighbour.y))
                        {
                            type = LevelManager.main.grid.GetTile(neighbour.x, neighbour.y);
                            visibility = grid.GetTile(neighbour.x, neighbour.y);
                            isReadyToVisible = visibility == TileVisibility.Visible || type == TileType.NULL;
                            if (showDebugLog) Debug.Log($"Checked neighbour {x},{y} ({visibility}) => {isReadyToVisible}");
                        }
                    }

                    if (isReadyToVisible)
                        grid.SetTile(x, y, TileVisibility.ReadyToVisible);
                } else
                    isReadyToVisible = grid.GetTile(x, y) != TileVisibility.ReadyToVisible;
            }

            return isReadyToVisible;
        }

        private void CheckTilesVisibilityAround(int x, int y)
        {
            TileType type = LevelManager.main.grid.GetTile(x, y);
            TileVisibility visibility = grid.GetTile(x, y);

            if (showDebugLog) Debug.Log($"Checking Tiles Visibility around {x},{y} ({visibility})");

            if (type != TileType.NULL && visibility == TileVisibility.Visible)
            {
                List<Vector2Int> neighbours = MyUtils.GatherNeighbours(x, y, 1, true, false);
                foreach (Vector2Int neighbour in neighbours)
                    IsNullTileReadyToVisible(neighbour.x, neighbour.y);
            } else if (type == TileType.NULL && visibility == TileVisibility.ReadyToVisible)
            {
                if (showDebugLog) Debug.Log("Checking for cluster completion");

                List<Vector2Int> cellsChecked = new List<Vector2Int>() { };
                bool clusterIsOk = CheckClusterTileVisibility(x, y, ref cellsChecked);
                if (clusterIsOk)
                    foreach (Vector2Int cell in cellsChecked)
                        DiscoverTile(cell.x, cell.y);
            }
        }

        private bool CheckClusterTileVisibility(int x, int y, ref List<Vector2Int> cellsChecked)
        {
            Vector2Int cell = new Vector2Int(x, y);

            if (!cellsChecked.Contains(cell))
                cellsChecked.Add(cell);

            bool result = grid.CellIsValid(x, y) && grid.GetTile(x, y) != TileVisibility.Invisible;

            if (showDebugLog) Debug.Log($"Checking cluster Tile {x},{y} ({grid.GetTile(x, y)}) = {result}");

            List<Vector2Int> neighbours = MyUtils.GatherNeighbours(x, y, 1, true, true);
            Vector2Int neighbour;
            for (int i = 0; i < neighbours.Count && result; i++)
            {
                neighbour = neighbours[i];
                if (!cellsChecked.Contains(neighbour))
                    if (grid.CellIsValid(neighbour.x, neighbour.y))
                        if (LevelManager.main.grid.GetTile(neighbour.x, neighbour.y) == TileType.NULL && grid.GetTile(neighbour.x, neighbour.y) != TileVisibility.Visible)
                            result = CheckClusterTileVisibility(neighbour.x, neighbour.y, ref cellsChecked);
            }

            return result;
        }
    }
}