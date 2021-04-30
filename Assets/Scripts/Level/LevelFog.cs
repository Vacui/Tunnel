﻿using PlayerLogic;
using System;
using System.Collections;
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


        [Header("Debug")]
        [EditorButton(nameof(DisableFog), "Disable Fog", activityType: ButtonActivityType.OnPlayMode), SerializeField] private bool showDebugLog = false;

        private void Awake() {
            if (main == null) main = this;
            else Destroy(this);

            tilemap = GetComponent<Tilemap>();
            grid = new GridXY<TileVisibility>();

            grid.OnTileChanged += (sender, args) => {
                if (showDebugLog) Debug.Log($"Setting Visibility Tile {args.x},{args.y} ({args.value})");

                switch (args.value) {
                    case TileVisibility.Invisible:
                        HiddenTile?.Invoke(this, new GridCoordsEventArgs { x = args.x, y = args.y });
                        break;
                    case TileVisibility.Visible:
                        if (LevelManager.main.Grid.GetTile(args.x, args.y) == Element.NULL && enabled)
                            StartCoroutine(SpawnNullTileSprite(args.x, args.y));
                        DiscoveredTile?.Invoke(this, new GridCoordsEventArgs { x = args.x, y = args.y });
                        break;
                }
                tilemap.SetTile(new Vector3Int(args.x, args.y, 0), args.value != TileVisibility.Visible ? visual : null);
                CheckTilesVisibilityAround(args.x, args.y);
            };

            LevelManager.main.Grid.OnGridCreated += (sender, args) => {
                tilemap.ClearAllTiles();
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
            bool isReadyToVisible = false;

            if (grid.CellIsValid(x, y)) {
                if (LevelManager.main.Grid.GetTile(x, y) == Element.NULL && grid.GetTile(x, y) == TileVisibility.Invisible) {
                    if (showDebugLog) Debug.Log($"Is Tile {x},{y} ReadyToVisible?");

                    isReadyToVisible = true;

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
                            if (showDebugLog) Debug.Log($"Checked neighbour {x},{y} ({visibility}) => {isReadyToVisible}");
                        }
                    }

                    if (isReadyToVisible) {
                        grid.SetTile(x, y, TileVisibility.ReadyToVisible);
                    }
                } else {
                    isReadyToVisible = grid.GetTile(x, y) != TileVisibility.ReadyToVisible;
                }
            }

            return isReadyToVisible;
        }

        private void CheckTilesVisibilityAround(int x, int y) {
            Element type = LevelManager.main.Grid.GetTile(x, y);
            TileVisibility visibility = grid.GetTile(x, y);

            if (showDebugLog) Debug.Log($"Checking Tiles Visibility around {x},{y} ({visibility})");

            if (type != Element.NULL && visibility == TileVisibility.Visible) {
                List<Vector2Int> neighbours = grid.GatherNeighbourCells(x, y, 1, true, false);
                foreach (Vector2Int neighbour in neighbours) {
                    IsNullTileReadyToVisible(neighbour.x, neighbour.y);
                }
            } else {
                if (type == Element.NULL && visibility == TileVisibility.ReadyToVisible) {
                    if (showDebugLog) Debug.Log("Checking for cluster completion");

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

            if (showDebugLog) Debug.Log($"Checking cluster Tile {x},{y} ({grid.GetTile(x, y)}) = {clusterIsNotInvisible}");

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
                    LeanTween.value(index, index + 1, clusterDiscoverySpeed).setOnComplete(() => { DiscoverNextClusterTile(index + 1, cells); });
                }
            }
        }

        IEnumerator SpawnNullTileSprite(int x, int y) {
            SpriteRenderer clusterTile = new GameObject().AddComponent<SpriteRenderer>();
            clusterTile.transform.position = tilemap.CellToWorld(new Vector3Int(x, y, 0));
            clusterTile.sprite = clusterTileVisual;
            clusterTile.color = tilemap.color;
            clusterTile.transform.localScale = Vector3.one;
            LeanTween.scale(clusterTile.gameObject, Vector3.zero, scaleTime).setDestroyOnComplete(true);
            yield return true;
        }
    }
}