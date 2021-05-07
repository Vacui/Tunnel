using PlayerLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level {
    [DisallowMultipleComponent, RequireComponent(typeof(Tilemap))]
    public class LevelFog : MonoBehaviour {
        public static LevelFog main { get; private set; }

        public enum TileVisibility {
            NULL,
            Invisible,
            ReadyToVisible,
            Visible
        }

        private GridXY<TileVisibility> grid;
        public static event EventHandler<GridCoordsEventArgs> HiddenTile;
        public static event EventHandler<GridCoordsEventArgs> DiscoveredTile;

        [Header("Visuals")]
        private Tilemap tilemap;
        [SerializeField, NotNull] private TileBase visual;
        [SerializeField, NotNull] private Sprite clusterTileVisual;
        [SerializeField] private float scaleTime = 1;
        [SerializeField] private float clusterDiscoverySpeed = 0.3f;
        private int clusterDiscoveryTweenId = -1;


        [System.Flags]
        enum LevelFogDebug {
            Nothing = 0,
            Setting_Tile = 1,
            Tiles_Around = 2,
            ReadyToVisible_Logic = 4,
            Cluster_Logic = 8,
            Everything = ~0
        }
        [Header("Debug")]
        [EditorButton(nameof(DisableFog), "Disable Fog", activityType: ButtonActivityType.OnPlayMode), SerializeField, EnumFlag] private LevelFogDebug showDebugLog;

        private void Awake() {
            if (main == null) main = this;
            else Destroy(this);

            tilemap = GetComponent<Tilemap>();
            grid = new GridXY<TileVisibility>();

            grid.OnTileChanged += (sender, args) => {
                if (showDebugLog.HasFlag(LevelFogDebug.Setting_Tile)) Debug.Log($"Setting Visibility Tile {args.x},{args.y} ({args.value})");

                switch (args.value) {
                    case TileVisibility.Invisible:
                        HiddenTile?.Invoke(this, new GridCoordsEventArgs { x = args.x, y = args.y });
                        break;
                    case TileVisibility.Visible:
                        DiscoveredTile?.Invoke(this, new GridCoordsEventArgs { x = args.x, y = args.y });
                        break;
                }
                tilemap.SetTile(new Vector3Int(args.x, args.y, 0), args.value != TileVisibility.Visible ? visual : null);
                CheckTilesVisibilityAround(args.x, args.y);
            };

            LevelManager.main.Grid.OnGridCreated += (sender, args) => {
                Debug.Log("Clearing Fog Tilemap");
                tilemap.ClearAllTiles();
                if (LeanTween.isTweening(clusterDiscoveryTweenId)) {
                    LeanTween.cancel(clusterDiscoveryTweenId, false);
                }
                grid.CreateGridXY(args.width, args.height, 1, Vector3.zero, false, TileVisibility.NULL, TileVisibility.NULL);
            };
            LevelManager.main.Grid.OnTileChanged += (sender, args) => {
                HideTile(args.x, args.y);
                if (args.value == Element.End) {
                    DiscoverTile(args.x, args.y);
                }
            };

            LevelManager.OnLevelReady += (sender, args) => {
                CheckNullTiles();
            };

            Player.Moved += (sender, args) => {
                DiscoverTile(args.x, args.y);
            };
            Player.StoppedMove += (sender, args) => {
                DiscoverTile(args.x + 1, args.y);
                DiscoverTile(args.x - 1, args.y);
                DiscoverTile(args.x, args.y + 1);
                DiscoverTile(args.x, args.y - 1);
            };
        }

        private void DiscoverTile(int x, int y) { grid.SetTile(x, y, TileVisibility.Visible); }
        private void HideTile(int x, int y) { grid.SetTile(x, y, TileVisibility.Invisible); }

        public void DisableFog() {
            grid.SetAllTiles(TileVisibility.Visible);
        }
        private void CheckNullTiles() {
            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    IsNullTileReadyToVisible(x, y);
                }
            }
        }

        private bool IsNullTileReadyToVisible(int x, int y) {

            if (!grid.CellIsValid(x, y)) {
                return false;
            }

            if (LevelManager.main.Grid.GetTile(x, y) != Element.NULL || grid.GetTile(x, y) != TileVisibility.Invisible) {
                return grid.GetTile(x, y) != TileVisibility.ReadyToVisible;
            }

            if (showDebugLog.HasFlag(LevelFogDebug.ReadyToVisible_Logic)) Debug.Log($"Is Tile {x},{y} ReadyToVisible?");

            bool isReadyToVisible = true;

            Element type;
            TileVisibility visibility;

            List<Vector2Int> neighbours = grid.GatherNeighbourCells(x, y, 1, true, false);
            Vector2Int neighbour;
            for (int i = 0; i < neighbours.Count && isReadyToVisible; i++) {
                neighbour = neighbours[i];
                if (LevelManager.main.Grid.CellIsValid(neighbour.x, neighbour.y) && grid.CellIsValid(neighbour.x, neighbour.y)) {
                    type = LevelManager.main.Grid.GetTile(neighbour.x, neighbour.y);
                    visibility = grid.GetTile(neighbour.x, neighbour.y);
                    isReadyToVisible = visibility == TileVisibility.Visible || type == Element.NULL;
                    if (showDebugLog.HasFlag(LevelFogDebug.ReadyToVisible_Logic)) Debug.Log($"Checked neighbour {x},{y} ({visibility}) => {isReadyToVisible}");
                }
            }

            if (isReadyToVisible) {
                grid.SetTile(x, y, TileVisibility.ReadyToVisible);
            }

            return isReadyToVisible;
        }

        private void CheckTilesVisibilityAround(int x, int y) {
            Element type = LevelManager.main.Grid.GetTile(x, y);
            TileVisibility visibility = grid.GetTile(x, y);

            if (showDebugLog.HasFlag(LevelFogDebug.Tiles_Around)) Debug.Log($"Checking Tiles Visibility around {x},{y} ({visibility})");

            if (type != Element.NULL && visibility == TileVisibility.Visible) {
                List<Vector2Int> neighbours = grid.GatherNeighbourCells(x, y, 1, true, false);
                foreach (Vector2Int neighbour in neighbours) {
                    IsNullTileReadyToVisible(neighbour.x, neighbour.y);
                }
            } else {
                if (type == Element.NULL && visibility == TileVisibility.ReadyToVisible) {
                    if (showDebugLog.HasFlag(LevelFogDebug.Cluster_Logic)) Debug.Log("Checking for cluster completion");

                    List<Vector2Int> cellsChecked = new List<Vector2Int>() { };
                    if (CheckClusterTileVisibility(x, y, ref cellsChecked)) {
                        DiscoverNextClusterTile(0, cellsChecked);
                    }
                }
            }
        }

        private bool CheckClusterTileVisibility(int x, int y, ref List<Vector2Int> cellsChecked) {
            Vector2Int cell = new Vector2Int(x, y);

            if (!cellsChecked.Contains(cell)) {
                cellsChecked.Add(cell);
            }

            bool clusterIsNotInvisible = grid.CellIsValid(x, y) && grid.GetTile(x, y) != TileVisibility.Invisible;

            if (showDebugLog.HasFlag(LevelFogDebug.Cluster_Logic)) Debug.Log($"Checking cluster Tile {x},{y} ({grid.GetTile(x, y)}) = {clusterIsNotInvisible}");

            List<Vector2Int> neighbours = grid.GatherNeighbourCells(x, y, 1, true, true);
            Vector2Int neighbour;
            for (int i = 0; i < neighbours.Count && clusterIsNotInvisible; i++) {
                neighbour = neighbours[i];
                if (!cellsChecked.Contains(neighbour)) {
                    if (grid.CellIsValid(neighbour.x, neighbour.y)) {
                        if (LevelManager.main.Grid.GetTile(neighbour.x, neighbour.y) == Element.NULL && grid.GetTile(neighbour.x, neighbour.y) != TileVisibility.Visible) {
                            clusterIsNotInvisible = CheckClusterTileVisibility(neighbour.x, neighbour.y, ref cellsChecked);
                        }
                    }
                }
            }

            return clusterIsNotInvisible;
        }

        private void DiscoverNextClusterTile(int index, List<Vector2Int> cells) {
            if (index < cells.Count) {
                DiscoverTile(cells[index].x, cells[index].y);
                if (index + 1 < cells.Count) {
                    clusterDiscoveryTweenId = LeanTween.value(index, index + 1, clusterDiscoverySpeed).setOnComplete(() => { DiscoverNextClusterTile(index + 1, cells); }).id;
                }
            }
        }
    }
}