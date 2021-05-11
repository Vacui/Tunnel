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

    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour {

        public static LevelManager Main { get; private set; }

        public const float CELLSIZE = 1f;

        public GridXY<Element> Grid { get; private set; }
        public Vector2Int StartCell { get; private set; }
        public Vector2Int EndCell { get; private set; }
        public enum LevelState { NotReady, NotPlayable, Ready, Playable, Win }
        public LevelState LvlState { get; private set; }

        public static event EventHandler OnLevelNotReady;
        public static event EventHandler OnLevelNotPlayable;
        public static event EventHandler<OnLevelReadyEventArgs> OnLevelReady;
        public class OnLevelReadyEventArgs : EventArgs {
            public int width, height;
        }
        public static event EventHandler<OnLevelPlayableEventArgs> OnLevelPlayable;
        public class OnLevelPlayableEventArgs : EventArgs {
            public int startX, startY;
            public int endX, endY;
        }

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
            Player.StoppedMoveStatic += (sender, args) => {
                if (Grid == null) {
                    return;
                }

                if (Grid.CellIsValid(args.x, args.y)) {
                    if (Grid.GetTile(args.x, args.y) == Element.End) {
                        LvlState = LevelState.Win;
                        OnWin?.Invoke();
                    }
                }
            };
        }

        public void ClearLevel() {
            LeanTween.cancelAll();
            StopAllCoroutines();

            Debug.Log("Level is not ready!");
            LvlState = LevelState.NotReady;
            OnLevelNotReady?.Invoke(this, null);
        }

        public void LoadLevel(GridXY<Element> newLevel) {
            if (newLevel == null || newLevel.Size == 0) {
                Debug.LogWarning("The new level grid is not valid");
                return;
            }

            Debug.Log($"0. Loading level {newLevel.Width}x{newLevel.Height}...");
            ClearLevel();

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

            LevelNavigation.SetUp(Grid.Width, Grid.Height, false, true, Grid);

            Debug.Log("Level is playable!");
            LvlState = LevelState.Playable;
            OnLevelPlayable?.Invoke(this, new OnLevelPlayableEventArgs { startX = StartCell.x, startY = StartCell.y, endX = EndCell.x, endY = EndCell.y });
            OnLevelStart?.Invoke();
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