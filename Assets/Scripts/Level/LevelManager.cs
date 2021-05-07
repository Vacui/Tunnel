using PlayerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Direction {
    NULL,
    All,
    Up,
    Right,
    Down,
    Left
}

public static class DirectionUtils {
    public static bool IsNull(this Direction dir) {
        return dir == Direction.NULL;
    }

    public static Direction Opposite(this Direction dir) {
        switch (dir) {
            default:
            case Direction.Up: return Direction.Down;
            case Direction.Right: return Direction.Left;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
        }
    }

    public static void ToOffset(this Direction dir, out int offsetX, out int offsetY) {
        offsetX = 0;
        offsetY = 0;
        switch (dir) {
            case Direction.Up: offsetY++; break;
            case Direction.Right: offsetX++; break;
            case Direction.Down: offsetY--; break;
            case Direction.Left: offsetX--; break;
        }
    }
    public static Vector2Int ToOffset(this Direction dir) {
        ToOffset(dir, out int offsetX, out int offsetY);
        return new Vector2Int(offsetX, offsetY);
    }

    public static Vector2Int FacingCell(this Direction dir, Vector2Int tileCell) {
        return tileCell + dir.ToOffset();
    }

    public static float ToAngle(this Direction dir) {
        float result = -1;

        if (dir != Direction.NULL)
            result = (int)dir * 90;

        return result;
    }

    public static Direction Rotate(this Direction direction, int angle) {
        switch (direction) {
            case Direction.NULL: return Direction.NULL;
            case Direction.All: return Direction.All;
            default:
                int dir = (int)direction;
                dir += angle / 90;
                if (dir > 5) dir -= 4;
                return (Direction)dir;
        }
    }
}

[Serializable]
public enum Element {
    NULL = 0,
    Start = 1,
    End = 2,
    Node = 3,
    Up = 4,
    Right = 5,
    Down = 6,
    Left = 7
}

public static class ElementUtils {
    public static Direction ToDirection(this Element element) {
        Direction result = Direction.NULL;
        switch (element) {
            case Element.NULL: result = Direction.NULL; break;
            case Element.Start: result = Direction.All; break;
            case Element.Node: result = Direction.All; break;
            case Element.End: result = Direction.All; break;
            case Element.Up: result = Direction.Up; break;
            case Element.Right: result = Direction.Right; break;
            case Element.Down: result = Direction.Down; break;
            case Element.Left: result = Direction.Left; break;
        }
        return result;
    }

    public static bool IsNodeType(this Element element) {
        return element.ToDirection() == Direction.All;
    }
}

namespace Level {
    public static class SeedUtils {
        public static string ToSeedString(this GridXY<Element> grid) {
            string result = "";
            if (grid.Width > 0 && grid.Height > 0) {
                result = $"{grid.Width}/{grid.Height}/";

                for (int y = 0; y < grid.Height; y++) {
                    for (int x = 0; x < grid.Width; x++) {
                        result += $"{((x != 0 || y != 0) ? "-" : "")}{(int)grid.GetTile(x, y)}";
                    }
                }

                result = result.TrimEnd('-');
            }
            return result;
        }

        public static LevelManager.Seed ToSeed(this GridXY<Element> grid) {
            return new LevelManager.Seed(grid.ToSeedString());
        }
    }


    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour {
        public class Seed {
            int width;
            public int Width { get { return width; } private set { width = value; } }
            int height;
            public int Height { get { return height; } private set { height = value; } }
            public int Size { get { return width * height; } }
            public List<int> Cells { get; private set; }
            public string SeedOriginal { get; private set; }

            public bool IsValid { get; private set; }

            public Seed(string seed) {
                Width = -1;
                Height = -1;
                Cells = new List<int>();

                SeedOriginal = seed;
                seed = seed.Trim();
                List<string> seedParts = seed.Split('/').ToList();

                IsValid = false;

                if (seedParts.Count != 3) {
                    Debug.LogWarning($"Error in seed number of parts [{seedParts.Count}]");
                    return;
                }

                if(!int.TryParse(seedParts[0], out width)) {
                    Debug.LogWarning($"Error in parsing seed Width [{seedParts[0]}]");
                    return;
                }

                if(Width <= 0) {
                    Debug.LogWarning($"Seed Width is less or equal to 0 [{Width}]");
                    return;
                }

                if(!int.TryParse(seedParts[1], out height)) {
                    Debug.LogWarning($"Error in parsing seed Height [{seedParts[1]}]");
                    return;
                }

                if (Height <= 0) {
                    Debug.LogWarning($"Seed Height is less or equal to 0 [{Height}]");
                    return;
                }

                if(seedParts[2].Count(c => (c == '-')) != (width * height) - 1) {
                    Debug.LogWarning($"Error in the seed cells section length [{seedParts[2].Count(c => (c == '-'))}]");
                    return;
                }


                List<string> cellString = seedParts[2].Split('-').ToList();
                for (int i = 0; i < cellString.Count; i++) {
                    if (cellString[i] != "" && int.TryParse(cellString[i], out int cell)) {
                        Cells.Add(cell);
                    } else {
                        Cells.Add(0);
                    }
                }
                IsValid = Cells.Count == cellString.Count;
            }

            public GridXY<Element> ToGrid() {
                GridXY<Element> grid = new GridXY<Element>();
                grid.CreateGridXY(Width, Height, 1, Vector3.zero, false, Element.NULL, Element.NULL);
                for(int i = 0; i < Cells.Count; i++) {
                    grid.SetTile(i, (Element)Cells[i]);
                }
                return grid;
            }

            public override string ToString() {
                string seed = $"{width}/{height}/";
                foreach (int cell in Cells) {
                    seed += $"{cell}-";
                }
                seed.Trim('-');
                return seed;
            }
        }

        public static LevelManager Main { get; private set; }

        public const float CELLSIZE = 1f;

        public GridXY<Element> Grid { get; private set; }
        public Vector2Int StartCell { get; private set; }
        public Vector2Int EndCell { get; private set; }
        public enum LevelState { NotReady, NotPlayable, Ready, Playable }
        public LevelState LvlState { get; private set; }

        public static event EventHandler OnLevelNotReady;
        public static event EventHandler OnLevelNotPlayable;
        public static event EventHandler<OnLevelReadyEventArgs> OnLevelReady;
        public class OnLevelReadyEventArgs : EventArgs { public int width, height; }
        public static event EventHandler<GridCoordsEventArgs> OnLevelPlayable;

        [Header("Events")]
        [SerializeField] private UnityEvent OnLevelStart;
        [SerializeField] private UnityEvent OnWin;

        [Header("Debug")]
        [SerializeField] private bool showDebugLog = false;

        private void Awake() {
            if (Main == null) Main = this;
            else Destroy(this);

            Grid = new GridXY<Element>();
        }

        private void OnEnable() {
            Player.StoppedMove += (sender, args) => {
                if (Grid == null) {
                    return;
                }

                if (Grid.CellIsValid(args.x, args.y)) {
                    if (Grid.GetTile(args.x, args.y) == Element.End) {
                        OnWin?.Invoke();
                    }
                }
            };
        }

        public void LoadLevel(GridXY<Element> newLevel) {
            if (newLevel == null || newLevel.Size == 0) {
                Debug.LogWarning("The new level grid is not valid");
                return;
            }

            Debug.Log($"0. Loading level {newLevel.Width}x{newLevel.Height}...");
            LeanTween.cancelAll();
            StopAllCoroutines();

            Debug.Log("Level is not ready!");
            LvlState = LevelState.NotReady;
            OnLevelNotReady?.Invoke(this, null);

            Debug.Log($"1. Initializing level...");
            Grid.CreateGridXY(newLevel.Width, newLevel.Height, 1, Vector3.zero, true, Element.NULL, Element.NULL);

            Debug.Log("Level is not playable!");
            LvlState = LevelState.NotPlayable;
            OnLevelNotPlayable?.Invoke(this, null);

            Debug.Log("2. Generating level...");
            StartCell = Vector2Int.one * -1;
            EndCell = Vector2Int.one * -1;

            bool hasStart = false;
            bool hasEnd = Grid.Size == 1;

            Element type;
            for (int x = 0; x < Grid.Width; x++) {
                for (int y = 0; y < Grid.Height; y++) {
                    type = newLevel.GetTile(x, y);
                    if (showDebugLog) Debug.Log($"Setted Tile {x},{y} ({type})");
                    Grid.SetTile(x, y, type);
                    if (type == Element.Start) {
                        StartCell = new Vector2Int(x, y);
                        hasStart = true;
                    } else {
                        if (type == Element.End) {
                            EndCell = new Vector2Int(x, y);
                            hasEnd = true;
                        }
                    }
                }
            }

            Debug.Log("Level is ready!");
            LvlState = LevelState.Ready;
            OnLevelReady?.Invoke(this, new OnLevelReadyEventArgs { width = Grid.Width, height = Grid.Height });

            if(!hasStart || !hasEnd) {
                Debug.LogWarning($"Level has {(!hasStart ? "NO": "")} Start and {(!hasEnd ? "NO" : "")} End");
                return;
            }

            Debug.Log("Level is playable!");
            LvlState = LevelState.Playable;
            OnLevelPlayable?.Invoke(this, new GridCoordsEventArgs { x = StartCell.x, y = StartCell.y });
            OnLevelStart?.Invoke();
        }
        public void LoadLevel(Seed lvlSeed) {
            if (lvlSeed == null || !lvlSeed.IsValid) {
                Debug.LogWarning($"Can't load level from seed {lvlSeed.SeedOriginal}");
                return;
            }

            Debug.Log($"Loading seed: {lvlSeed}");
            LoadLevel(lvlSeed.ToGrid());
        }
        public void LoadLevel(string seed) {
            LoadLevel(new Seed(seed));
        }

        public void Restart() {
            GridXY<Element> newLevel = new GridXY<Element>();
            newLevel.CreateGridXY(Grid.Width, Grid.Height, Grid.CellSize, Grid.OriginPosition, false, Element.NULL, Element.NULL);
            for (int x = 0; x < Grid.Width; x++) {
                for (int y = 0; y < Grid.Height; y++) {
                    newLevel.SetTile(x, y, Grid.GetTile(x, y));
                }
            }
            LoadLevel(newLevel);
        }
    }
}