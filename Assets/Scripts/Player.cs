using Level;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerLogic
{
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        public static Player main;

        public static event EventHandler<GridCoordsEventArgs> Moved;
        public static event EventHandler<GridCoordsEventArgs> StoppedMove;
        public static event EventHandler<PlayerInputEventArgs> Input;
        public class PlayerInputEventArgs : EventArgs { public int moves; }

        [SerializeField, Disable] private int x = -1;
        [SerializeField, Disable] private int y = -1;
        [SerializeField, Disable] private bool isSafe = true;
        private bool IsSafe
        {
            get { return isSafe; }
            set
            {
                isSafe = value;
                if (isSafe)
                {
                    character.transform.localScale = smallScale;
                    LeanTween.scale(character, bigScale, SCALE_TIME);
                    StoppedMove?.Invoke(this, new GridCoordsEventArgs { x = x, y = y });
                }
            }
        }

        [Header("Movement")]
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("moveSpeed")] private float moveTime = 0.3f;
        [SerializeField, Disable] private int moves;
        public int Moves
        {
            get { return moves; }
            private set
            {
                moves = value;
                Input?.Invoke(this, new PlayerInputEventArgs { moves = value });
            }
        }
        private Direction dirCurrent = Direction.NULL;

        [Header("Character")]
        [SerializeField, Disable] bool isActive = true;
        public bool IsActive
        {
            get { return isActive; }
            private set
            {
                isActive = value;
                character.SetActive(isActive);
            }
        }
        [SerializeField, NotNull] private GameObject character;

        private const float SCALE_SIZE = 0.7f;
        private Vector3 bigScale { get { return Vector3.one; } }
        private Vector3 smallScale { get { return Vector3.one * SCALE_SIZE; } }
        private const float SCALE_TIME = 0.1f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLog = false;

        EventHandler<GridCoordsEventArgs> playerSpawn = null;

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);
            IsActive = false;
            IsSafe = false;
        }

        private void OnEnable()
        {
            LevelManager.OnLevelNotReady += (sender, args) => IsActive = false;

            playerSpawn = delegate (object sender, GridCoordsEventArgs args)
            {
                IsActive = true;
                MoveToStartCell(args.x, args.y);
            };
            LevelManager.OnLevelPlayable += playerSpawn;
        }

        private void OnDisable()
        {
            LevelManager.OnLevelNotReady += (sender, args) => IsActive = false;
            LevelManager.OnLevelPlayable -= playerSpawn;
        }

        private void Update()
        {
            if (IsSafe && IsActive)
            {
                Direction moveDirection = Direction.NULL;
                if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)) moveDirection = Direction.Up;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow)) moveDirection = Direction.Left;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)) moveDirection = Direction.Down;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)) moveDirection = Direction.Right;

                if (moveDirection != Direction.NULL)
                {
                    Moves++;
                    MoveToCell(moveDirection);
                }
            }
        }

        private void MoveToCell(int x, int y, bool teleport)
        {
            if (LevelManager.main.grid != null && IsActive)
                if (LevelManager.main.grid.CellIsValid(x, y))
                {
                    if (LevelManager.main.grid.GetTile(x, y) != Element.NULL)
                    {
                        if (showDebugLog) Debug.Log($"Moving to tile {x},{y}", gameObject);

                        this.x = x;
                        this.y = y;

                        if (teleport)
                        {
                            transform.position = LevelVisual.main.Tilemap.CellToWorld(new Vector3Int(x, y, 0));
                            CheckCurrentTile();
                        } else
                        {
                            if (IsSafe)
                            {
                                character.transform.localScale = bigScale;
                                LeanTween.scale(character, smallScale, SCALE_TIME);
                            }
                            LeanTween.move(gameObject, LevelVisual.main.Tilemap.CellToWorld(new Vector3Int(x, y, 0)), moveTime).setOnComplete(() => CheckCurrentTile());
                        }
                        Moved?.Invoke(this, new GridCoordsEventArgs { x = x, y = y });
                    } else
                    {
                        Debug.LogWarning($"Can't move to NULL tile {x},{y}.", gameObject);
                        IsSafe = true;
                    }
                } else
                {
                    Debug.LogWarning($"Can't move to non valid cell {x},{y}.", gameObject);
                    IsSafe = true;
                }
        }

        private void MoveToCell(Direction dir)
        {
            Direction dirCurrentTile = LevelManager.main.grid.GetTile(x, y).ToDirection();
            if (dir != Direction.NULL)
            {
                if (dirCurrentTile == Direction.All || dirCurrentTile == dir)
                {
                    dirCurrent = dir;
                    dirCurrent.ToOffset(out int offsetX, out int offsetY);

                    MoveToCell(x + offsetX, y + offsetY, false);
                } else
                {
                    if (showDebugLog) Debug.Log($"Character can't exit tile with direction {dirCurrentTile} to {dir}.", gameObject);
                    if (!IsSafe)
                        MoveToCell(dirCurrentTile);
                }
            } else
            {
                Debug.LogWarning($"Character can't move in a null direction.", gameObject);
                IsSafe = true;
            }
        }

        public void MoveToStartCell(int x, int y)
        {
            IsActive = true;
            Moves = 0;
            dirCurrent = Direction.NULL;
            MoveToCell(x, y, true);
        }

        private void MoveToCurrentDirection()
        {
            if (showDebugLog) Debug.Log($"Moving To Current Direction {dirCurrent}.", gameObject);
            MoveToCell(dirCurrent);
        }

        public void CheckCurrentTile()
        {
            if (showDebugLog) Debug.Log("Checking current tile", gameObject);
            if (LevelManager.main.grid.CellIsValid(x, y))
            {
                Element currentTileType = LevelManager.main.grid.GetTile(x, y);
                if (currentTileType != Element.NULL)
                {
                    IsSafe = currentTileType.ToDirection() == Direction.All;
                    if (!IsSafe) MoveToCurrentDirection();
                } else
                {
                    Debug.LogError($"NULL tile type {x},{y}.", gameObject);
                    IsSafe = true;
                }
            }
        }
    }
}