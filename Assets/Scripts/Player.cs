using Level;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    public static Player main;

    public static event EventHandler<GridCoordsEventArgs> OnPlayerStartedMove;
    public static event EventHandler<GridCoordsEventArgs> OnPlayerStoppedMove;
    public static event EventHandler<OnPlayerInputEventArgs> OnPlayerInput;
    public class OnPlayerInputEventArgs : EventArgs { public int moves; }

    [SerializeField, Disable] bool isActive = true;
    public bool IsActive
    {
        get { return isActive; }
        private set {
            isActive = value;
            if (isActive) ShowVisual();
            else HideVisual();
        }
    }

    [SerializeField, Disable] private int x = -1;
    [SerializeField, Disable] private int y = -1;
    [SerializeField, Disable] private bool isSafe = true;
    private bool IsSafe
    {
        get { return isSafe; }
        set
        {
            isSafe = value;
            if(isSafe)
                OnPlayerStoppedMove?.Invoke(this, new GridCoordsEventArgs { x = x, y = y });
        }
    }

    [Header("Movement")]
    [SerializeField, Disable] private int moves;
    public int Moves
    {
        get { return moves; }
        private set {
            moves = value;
            OnPlayerInput?.Invoke(this, new OnPlayerInputEventArgs { moves = value });
        }
    }
    private Direction dirCurrent = Direction.NULL;    
    private const float SCALE_SIZE = 1.2f;
    private const float SCALE_SPEED = 0.1f;
    private int currentScaleTweenId;

    [Header("Visual")]
    [SerializeField] private ElementsVisuals visuals;

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = false;

    EventHandler<GridCoordsEventArgs> playerSpawn = null;

    private void Awake()
    {
        if (main == null) main = this;
        else Destroy(this);
        IsActive = false;
    }

    private void OnEnable()
    {
        LevelManager.OnLevelNotReady += (sender, args) => { IsActive = false; };

        playerSpawn = delegate(object sender, GridCoordsEventArgs args) {
            IsActive = true;
            MoveToStartCell(args.x, args.y);
        };
        LevelManager.OnLevelPlayable += playerSpawn;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelNotReady += (sender, args) => { IsActive = false; };
        LevelManager.OnLevelPlayable -= playerSpawn;
    }

    private void Update()
    {
        if (IsSafe && IsActive)
        {
            Direction moveDirection = Direction.NULL;
            if (Input.GetKeyDown(KeyCode.UpArrow)) moveDirection = Direction.Up;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) moveDirection = Direction.Left;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) moveDirection = Direction.Down;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) moveDirection = Direction.Right;

            if (moveDirection != Direction.NULL)
            {
                Moves++;
                MoveToCell(moveDirection);
            }
        }
    }

    private void HideVisual() { GetComponent<SpriteRenderer>().enabled = false; }
    private void ShowVisual() { GetComponent<SpriteRenderer>().enabled = true; }

    private void MoveToCell(int x, int y, bool teleport)
    {
        if (LevelManager.main.grid != null)
        {
            if (LevelManager.main.grid.CellIsValid(x, y))
            {
                if (LevelManager.main.grid.GetTile(x, y) != TileType.NULL)
                {
                    if (showDebugLog) Debug.Log($"Moving to tile {x},{y}", gameObject);

                    Vector2 nextPos = LevelManager.main.grid.CellToWorld(x, y);

                    this.x = x;
                    this.y = y;

                    CancelMovementTween();
                    transform.localPosition = nextPos;

                    if (visuals != null)
                    {
                        ElementsVisuals.VisualData visualData = visuals.GetVisualData(LevelManager.main.grid.GetTile(x, y));
                        GetComponent<SpriteRenderer>().sprite = visualData.sprite;
                    }

                    if (teleport)
                        CheckCurrentTile();
                    else
                    {
                        transform.localScale = Vector3.one * SCALE_SIZE;
                        currentScaleTweenId = LeanTween.scale(gameObject, Vector3.one, SCALE_SPEED).setOnComplete(() => CheckCurrentTile()).id;
                    }


                    OnPlayerStartedMove?.Invoke(this, new GridCoordsEventArgs { x = x, y = y });
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
                if(showDebugLog) Debug.Log($"Character can't exit tile with direction {dirCurrentTile} to {dir}.", gameObject);
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

    private void CheckCurrentTile()
    {
        if (showDebugLog) Debug.Log("Checking current tile", gameObject);
        if (LevelManager.main.grid.CellIsValid(x, y))
        {
            TileType currentTileType = LevelManager.main.grid.GetTile(x, y);
            if (currentTileType != TileType.NULL)
            {
                IsSafe = currentTileType.ToDirection() == Direction.All;
                if (!IsSafe)
                    MoveToCurrentDirection();
                else
                    CancelMovementTween();
            } else
            {
                Debug.LogError($"NULL tile type {x},{y}.", gameObject);
                IsSafe = true;
            }
        }
    }

    private void CancelMovementTween()
    {
        if (showDebugLog) Debug.Log("Cancelling movement tween.", gameObject);
        if (currentScaleTweenId > 0 && LeanTween.isTweening(currentScaleTweenId))
            LeanTween.cancel(currentScaleTweenId);
        transform.localScale = Vector3.one;
    }
}