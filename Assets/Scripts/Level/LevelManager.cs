﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    public static class SeedUtils
    {
        public static string ToSeedString(this GridXY<TileType> grid)
        {
            string result = "";
            if (grid.width > 0 && grid.height > 0)
            {
                result = $"{grid.width}/{grid.height}/";

                for (int x = 0; x < grid.width; x++)
                    for (int y = 0; y < grid.height; y++)
                        result += $"{((x != 0 || y != 0) ? "-" : "")}{(int)grid.GetTile(x, y)}";
            }
            return result;
        }
        public static LevelManager.Seed ToSeed(this GridXY<TileType> grid) { return new LevelManager.Seed(grid.ToSeedString()); }
    }


    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour
    {
        public class Seed
        {
            int width;
            public int Width { get { return width; } }
            int height;
            public int Height { get { return height; } }
            public int Size { get { return width * height; } }
            public List<int> cells { get; private set; }
            public string SeedOriginal { get; private set; }

            public bool isValid { get; private set; }

            public Seed(string seed)
            {
                width = -1;
                height = -1;
                cells = new List<int>();

                SeedOriginal = seed;
                seed = seed.Trim();
                List<string> seedParts = seed.Split('/').ToList();

                isValid = false;

                if (seedParts.Count == 3)
                {
                    if (int.TryParse(seedParts[0], out width))
                    {
                        if (width > 0)
                        {
                            if (int.TryParse(seedParts[1], out height))
                            {
                                if (height > 0)
                                {
                                    if (seedParts[2].Count(c => (c == '-')) == (width * height) - 1)
                                    {
                                        List<string> cellString = seedParts[2].Split('-').ToList();
                                        int cell;
                                        for (int i = 0; i < cellString.Count; i++)
                                        {
                                            if (cellString[i] != "" && int.TryParse(cellString[i], out cell))
                                                cells.Add(cell);
                                            else
                                                cells.Add(0);
                                        }
                                        isValid = cells.Count == cellString.Count;
                                    } else { Debug.LogWarning("Error in the seed cells section length."); }
                                } else { Debug.LogWarning("Seed height is less or equal to 0."); }
                            } else { Debug.LogWarning("Error in parsing seed height number."); }
                        } else { Debug.LogWarning("Seed width is less or equal to 0."); }
                    } else { Debug.LogWarning("Error in parsing seed width number."); }
                } else { Debug.LogWarning("Error in seed number of parts."); }
            }

            public override string ToString()
            {
                string seed = $"{width}/{height}/";
                foreach (int cell in cells)
                {
                    seed += $"{cell}-";
                }
                seed.Trim('-');
                return seed;
            }
        }

        public static LevelManager main;

        public const float CELLSIZE = 1.1f;

        public GridXY<TileType> grid { get; private set; }
        public enum LevelState { NotReady, NotPlayable, Ready, Playable }
        public LevelState LvlState { get; private set; }

        public static event EventHandler OnLevelNotReady;
        public static event EventHandler OnLevelNotPlayable;
        public static event EventHandler<OnLevelReadyEventArgs> OnLevelReady;
        public class OnLevelReadyEventArgs : EventArgs { public int width, height; }
        public static event EventHandler<GridCoordsEventArgs> OnLevelPlayable;

        [Header("Events")]
        [SerializeField] private UnityEvent OnWin;

        [Header("Debug")]
        [SerializeField] private bool showDebugLog = false;

        private void Awake()
        {
            grid = new GridXY<TileType>();
            if (main == null) main = this;
            else Destroy(this);
        }

        private void OnEnable()
        {
            Player.OnPlayerStoppedMove += (sender, args) =>
            {
                if (grid != null)
                    if (grid.CellIsValid(args.x, args.y))
                        if (grid.GetTile(args.x, args.y) == TileType.Goal)
                            OnWin?.Invoke();
            };
        }

        public void LoadLevel(Seed lvlSeed)
        {
            if (lvlSeed != null && lvlSeed.isValid)
            {
                Debug.Log("0. Loading level...");

                LeanTween.cancelAll();
                StopAllCoroutines();

                Debug.Log("Level is not ready!");
                LvlState = LevelState.NotReady;
                OnLevelNotReady?.Invoke(this, null);

                Debug.Log($"1. Initializing level...");
                grid.CreateGridXY(lvlSeed.Width, lvlSeed.Height, CELLSIZE, new Vector2(lvlSeed.Width / 2.0f - 0.5f, lvlSeed.Height / 2.0f - 0.5f) * new Vector2(-1, 1) * CELLSIZE);
                grid.SetAllTiles(TileType.NULL);

                Debug.Log("Level is not playable!");
                LvlState = LevelState.NotPlayable;
                OnLevelNotPlayable?.Invoke(this, null);

                Debug.Log("2. Generating level...");
                Vector2Int startPos = Vector2Int.one * -1;

                bool hasStart = false, hasEnd = lvlSeed.Size == 1;

                TileType type;
                for (int i = 0; i < lvlSeed.cells.Count; i++)
                {
                    grid.CellNumToCell(i, out int x, out int y);
                    type = (TileType)lvlSeed.cells[i];
                    grid.SetTile(x, y, type);
                    if (showDebugLog) Debug.Log($"Setted Tile n.{i} {grid.GetTileToString(x, y)}");
                    if (type == TileType.Player)
                    {
                        startPos.x = x;
                        startPos.y = y;
                        hasStart = true;
                    } else
                        hasEnd = hasEnd || type == TileType.Goal;
                }

                Debug.Log("Level is ready!");
                LvlState = LevelState.Ready;
                OnLevelReady?.Invoke(this, new OnLevelReadyEventArgs { width = lvlSeed.Width, height = lvlSeed.Height });
                if (hasStart && hasEnd)
                {
                    Debug.Log("Level is playable!");
                    LvlState = LevelState.Playable;
                    OnLevelPlayable?.Invoke(this, new GridCoordsEventArgs { x = startPos.x, y = startPos.y });
                }
            } else
                Debug.LogWarning($"Can't load level from seed {lvlSeed.SeedOriginal}");
        }
        public void LoadLevel(string seed) { LoadLevel(new Seed(seed)); }
    }
}