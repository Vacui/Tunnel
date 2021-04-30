using PlayerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Direction
{
    NULL,
    All,
    Up,
    Right,
    Down,
    Left
}

public static class DirectionUtils
{
    public static bool IsNull(this Direction dir)
    {
        return dir == Direction.NULL;
    }

    public static Direction Opposite(this Direction dir)
    {
        switch (dir)
        {
            default:
            case Direction.Up: return Direction.Down;
            case Direction.Right: return Direction.Left;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
        }
    }

    public static void ToOffset(this Direction dir, out int offsetX, out int offsetY)
    {
        offsetX = 0;
        offsetY = 0;
        switch (dir)
        {
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

    public static float ToAngle(this Direction dir)
    {
        float result = -1;

        if (dir != Direction.NULL)
            result = (int)dir * 90;

        return result;
    }

    public static Direction Rotate(this Direction direction, int angle)
    {
        switch (direction)
        {
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
public enum Element
{
    NULL = 0,
    Start = 1,
    End = 2,
    Node = 3,
    Up = 4,
    Right = 5,
    Down = 6,
    Left = 7
}

public static class ElementUtils
{
    public static Direction ToDirection(this Element tileType)
    {
        Direction result = Direction.NULL;
        switch (tileType)
        {
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
            public int Width { get { return width; } }
            int height;
            public int Height { get { return height; } }
            public int Size { get { return width * height; } }
            public List<int> cells { get; private set; }
            public string SeedOriginal { get; private set; }

            public bool isValid { get; private set; }

            public Seed(string seed) {
                width = -1;
                height = -1;
                cells = new List<int>();

                SeedOriginal = seed;
                seed = seed.Trim();
                List<string> seedParts = seed.Split('/').ToList();

                isValid = false;

                if (seedParts.Count == 3) {
                    if (int.TryParse(seedParts[0], out width)) {
                        if (width > 0) {
                            if (int.TryParse(seedParts[1], out height)) {
                                if (height > 0) {
                                    if (seedParts[2].Count(c => (c == '-')) == (width * height) - 1) {
                                        List<string> cellString = seedParts[2].Split('-').ToList();
                                        for (int i = 0; i < cellString.Count; i++) {
                                            if (cellString[i] != "" && int.TryParse(cellString[i], out int cell)) {
                                                cells.Add(cell);
                                            } else {
                                                cells.Add(0);
                                            }
                                        }
                                        isValid = cells.Count == cellString.Count;
                                    } else Debug.LogWarning($"Error in the seed cells section length [{seedParts[2].Count(c => (c == '-'))}]");
                                } else Debug.LogWarning($"Seed height is less or equal to 0 [{height}]");
                            } else Debug.LogWarning($"Error in parsing seed height number [{seedParts[1]}]");
                        } else Debug.LogWarning($"Seed width is less or equal to 0 [{width}]");
                    } else Debug.LogWarning($"Error in parsing seed width number [{seedParts[0]}]");
                } else Debug.LogWarning($"Error in seed number of parts [{seedParts.Count}]");
            }

            public override string ToString() {
                string seed = $"{width}/{height}/";
                foreach (int cell in cells) {
                    seed += $"{cell}-";
                }
                seed.Trim('-');
                return seed;
            }
        }

        public static LevelManager main { get; private set; }

        public const float CELLSIZE = 1f;

        public GridXY<Element> Grid { get; private set; }
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
            if (main == null) main = this;
            else Destroy(this);

            Grid = new GridXY<Element>();
        }

        private void OnEnable() {
            Player.StoppedMove += (sender, args) => {
                if (Grid != null) {
                    if (Grid.CellIsValid(args.x, args.y)) {
                        if (Grid.GetTile(args.x, args.y) == Element.End) {
                            OnWin?.Invoke();
                        }
                    }
                }
            };
        }

        public void LoadLevel(Seed lvlSeed) {
            if (lvlSeed != null && lvlSeed.isValid) {
                Debug.Log("0. Loading level...");
                Debug.Log($"   Seed: {lvlSeed}");

                LeanTween.cancelAll();
                StopAllCoroutines();

                Debug.Log("Level is not ready!");
                LvlState = LevelState.NotReady;
                OnLevelNotReady?.Invoke(this, null);

                Debug.Log($"1. Initializing level...");
                Grid.CreateGridXY(lvlSeed.Width, lvlSeed.Height, 1, Vector3.zero, true, Element.NULL, Element.NULL);

                Debug.Log("Level is not playable!");
                LvlState = LevelState.NotPlayable;
                OnLevelNotPlayable?.Invoke(this, null);

                Debug.Log("2. Generating level...");
                Vector2Int startPos = Vector2Int.one * -1;

                bool hasStart = false, hasEnd = lvlSeed.Size == 1;

                Element type;
                for (int i = 0; i < lvlSeed.cells.Count; i++) {
                    Grid.CellNumToCell(i, out int x, out int y);
                    type = (Element)lvlSeed.cells[i];
                    Grid.SetTile(x, y, type);
                    if (showDebugLog) Debug.Log($"Setted Tile n.{i} {Grid.GetTileToString(x, y)}");
                    if (type == Element.Start) {
                        startPos.x = x;
                        startPos.y = y;
                        hasStart = true;
                    } else {
                        hasEnd = hasEnd || type == Element.End;
                    }
                }

                Debug.Log("Level is ready!");
                LvlState = LevelState.Ready;
                OnLevelReady?.Invoke(this, new OnLevelReadyEventArgs { width = lvlSeed.Width, height = lvlSeed.Height });
                if (hasStart && hasEnd) {
                    Debug.Log("Level is playable!");
                    LvlState = LevelState.Playable;
                    OnLevelPlayable?.Invoke(this, new GridCoordsEventArgs { x = startPos.x, y = startPos.y });
                    OnLevelStart?.Invoke();
                } else {
                    Debug.Log($"Level Start={hasStart}, End={hasEnd}");
                }
            } else Debug.LogWarning($"Can't load level from seed {lvlSeed.SeedOriginal}");
        }
        public void LoadLevel(string seed) {
            LoadLevel(new Seed(seed));
        }
    }
}