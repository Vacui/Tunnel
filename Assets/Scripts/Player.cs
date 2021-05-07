using Level;
using System;
using UnityEngine;

namespace PlayerLogic {
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour {
        public static event EventHandler<GridCoordsEventArgs> StartedMoveStatic;
        public static event EventHandler<GridCoordsEventArgs> MovedStatic;
        public static event EventHandler<GridCoordsEventArgs> StoppedMoveStatic;

        [SerializeField, Disable] private int x = -1;
        [SerializeField, Disable] private int y = -1;
        [SerializeField, Disable] private bool isSafe = true;
        private bool IsSafe {
            get { return isSafe; }
            set {
                bool changedValue = isSafe != value;
                isSafe = value;
                if (changedValue && isSafe) {
                    StoppedMoveStatic?.Invoke(this, new GridCoordsEventArgs { x = x, y = y });
                    StoppedMove?.Invoke(new Vector3(x, y, 0));
                } else {
                    if (changedValue && !isSafe) {
                        StartedMoveStatic?.Invoke(this, new GridCoordsEventArgs { x = x, y = y });
                        StartedMove?.Invoke(new Vector3(x, y, 0));
                    }
                }
            }
        }

        [Header("Movement")]
        [SerializeField] private float moveTime = 0.3f;
        [SerializeField] private float wallBounceTime = 0.3f;
        [SerializeField, Clamp(0, 1)] public float wallBounceDistance = 0.3f;
        private Direction dirCurrent = Direction.NULL;

        [Header("Character")]
        [EditorButton(nameof(DisablePlayer), "Disable Player", activityType: ButtonActivityType.OnPlayMode), SerializeField, Disable] bool isActive = true;
        public bool IsActive {
            get { return isActive; }
            private set {
                isActive = value;
                character.gameObject.SetActive(isActive);
            }
        }
        [SerializeField, NotNull] private SpriteRenderer character;
        [SerializeField, NotNull] private GameObject characterBody;

        private const float SCALE_SIZE = 0.7f;
        private Vector3 bigScale { get { return Vector3.one; } }
        private Vector3 smallScale { get { return Vector3.one * SCALE_SIZE; } }
        private const float SCALE_TIME = 0.1f;

        [Header("Events")]
        [SerializeField] private UltEventVector3 StartedMove;
        [SerializeField] private UltEventVector3 Moved;
        [SerializeField] private UltEventVector3 StoppedMove;

        [Header("Debug")]
        [SerializeField] private bool showDebugLog = false;

        private void Awake() {
            IsActive = false;
            IsSafe = false;

            LevelManager.OnLevelNotReady += (sender, args) => {
                IsActive = false;
            };
            LevelManager.OnLevelPlayable += (sender, args) => {
                MoveToStartCell(args.startX, args.startY);
            };

            StartedMoveStatic += (sender, args) => {
                character.transform.localScale = bigScale;
                LeanTween.scale(character.gameObject, smallScale, SCALE_TIME);
            };
            StoppedMoveStatic += (sender, args) => {
                character.transform.localScale = smallScale;
                LeanTween.scale(character.gameObject, bigScale, SCALE_TIME);
            };
        }

        private void Update() {
            if (IsSafe && IsActive) {
                Direction moveDirection = Direction.NULL;
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    moveDirection = Direction.Up;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    moveDirection = Direction.Left;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    moveDirection = Direction.Down;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    moveDirection = Direction.Right;
                }

                if (moveDirection != Direction.NULL) {
                    MoveToCell(moveDirection);
                }
            }
        }

        private void MoveToCell(int newX, int newY, bool teleport) {
            bool canMove = false;

            if(LevelManager.Main.Grid == null) {
                Debug.LogWarning("Level Manager grid is null", gameObject);
                return;
            }

            if (!IsActive) {
                if (showDebugLog) Debug.LogWarning("The player is not active", gameObject);
                return;
            }

            if (LevelManager.Main.Grid.CellIsValid(newX, newY)) {
                Element newTileElement = LevelManager.Main.Grid.GetTile(newX, newY);
                if (newTileElement != Element.NULL) {
                    newTileElement.ToDirection().ToOffset(out int offsetX, out int offsetY);
                    if (x != newX + offsetX || y != newY + offsetY || (x == newX && y == newY) || newTileElement.ToDirection() == Direction.All) {
                        if (showDebugLog) Debug.Log($"Moving to tile {newX},{newY}", gameObject);

                        x = newX;
                        y = newY;

                        canMove = true;
                    } else Debug.LogWarning($"The tile {newX},{newY} is looking to current player tile", gameObject);
                } else Debug.LogWarning($"Can't move to NULL tile {newX},{newY}.", gameObject);
            } else Debug.LogWarning($"Can't move to non valid cell {newX},{newY}.", gameObject);

            IsSafe = false;

            if (canMove && teleport) {
                transform.position = LevelVisual.main.Tilemap.CellToWorld(new Vector3Int(newX, newY, 0));
                CheckCurrentTile();
            } else {
                MoveAnim(LevelVisual.main.Tilemap.CellToWorld(new Vector3Int(newX, newY, 0)), !canMove);
            }

            if (canMove) {
                MovedStatic?.Invoke(this, new GridCoordsEventArgs { x = newX, y = newY });
                Moved?.Invoke(new Vector3(newX, newY, 0));
            }
        }

        private void MoveToCell(Direction dir) {
            Direction dirCurrentTile = LevelManager.Main.Grid.GetTile(x, y).ToDirection();

            if (dir == Direction.NULL) {
                Debug.LogWarning($"Character can't move in a null direction.", gameObject);
                IsSafe = true;
                return;
            }

            if (dirCurrentTile != Direction.All && dirCurrentTile != dir) {
                if (showDebugLog) Debug.Log($"Character can't exit tile with direction {dirCurrentTile} to {dir}.", gameObject);
                if (!IsSafe) {
                    MoveToCell(dirCurrentTile);
                }
                return;
            }

            dirCurrent = dir;
            dirCurrent.ToOffset(out int offsetX, out int offsetY);

            MoveToCell(x + offsetX, y + offsetY, false);
        }

        public void MoveToStartCell(int x, int y) {
            IsActive = true;
            dirCurrent = Direction.NULL;
            if (showDebugLog) Debug.Log($"Moving to start cell {x},{y}");
            MoveToCell(x, y, true);
        }

        private void MoveToCurrentDirection() {
            if (showDebugLog) Debug.Log($"Moving To Current Direction {dirCurrent}.", gameObject);
            MoveToCell(dirCurrent);
        }

        public void CheckCurrentTile() {
            if (showDebugLog) Debug.Log("Checking current tile", gameObject);

            if (!LevelManager.Main.Grid.CellIsValid(x, y)) {
                return;
            }

            Element currentTileType = LevelManager.Main.Grid.GetTile(x, y);

            if(currentTileType == Element.NULL) {
                Debug.LogError($"NULL tile type {x},{y}.", gameObject);
                IsSafe = true;
                return;
            }

            IsSafe = currentTileType.ToDirection() == Direction.All;
            if (!IsSafe) MoveToCurrentDirection();
        }

        private void MoveAnim(Vector3 position, bool againstWall) {
            float distancePercentage = againstWall ? wallBounceDistance : 1;

            Vector3 positionDiff = new Vector3(transform.position.x - position.x, transform.position.y - position.y);
            positionDiff = positionDiff * distancePercentage;

            LTDescr moveLTDescr = LeanTween.move(againstWall ? characterBody : gameObject, transform.position - positionDiff, moveTime);
            if (againstWall) {
                moveLTDescr.setTime(wallBounceTime);
                moveLTDescr.setLoopPingPong(1).setOnComplete(() => IsSafe = true);
            } else {
                moveLTDescr.setOnComplete(() => CheckCurrentTile());
            }
        }

        private void DisablePlayer() {
            IsActive = false;
        }
    }
}