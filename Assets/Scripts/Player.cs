using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    public static event EventHandler<GridCoordsEventArgs> OnPlayerStartedMove;
    public class GridCoordsEventArgs : EventArgs
    {
        public int x, y;
    }
    public static event EventHandler<GridCoordsEventArgs> OnPlayerStoppedMove;

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
    private Direction dirCurrent = Direction.NULL;
    [SerializeField] private /*const*/ float SCALE_SIZE = 1.2f;
    [SerializeField] private /*const*/ float SCALE_SPEED = 0.5f;
    private int currentScaleTweenId;

    [Header("Visual")]
    [SerializeField] private ElementsVisuals visuals;

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = false;

    public Player(int x, int y)
    {
        this.x = x;
        this.y = y;
        MoveToStartCell(x, y);
    }

    private void Update()
    {
        if (IsSafe)
        {
            if (Input.GetKeyDown(KeyCode.W)) MoveToCell(Direction.Up);
            else if (Input.GetKeyDown(KeyCode.A)) MoveToCell(Direction.Left);
            else if (Input.GetKeyDown(KeyCode.S)) MoveToCell(Direction.Down);
            else if (Input.GetKeyDown(KeyCode.D)) MoveToCell(Direction.Right);
        }
    }

    private void MoveToCell(int x, int y, bool teleport)
    {
        if (LevelManager.gridLevel != null)
        {
            if (LevelManager.gridLevel.CellIsValid(x, y))
            {
                if (LevelManager.gridLevel.GetTile(x, y) != TileType.NULL)
                {
                    if (showDebugLog) Debug.Log($"Moving to tile {x},{y}", gameObject);

                    Vector2 nextPos = LevelManager.gridLevel.CellToWorld(x, y);

                    this.x = x;
                    this.y = y;

                    CancelMovementTween();
                    transform.localPosition = nextPos;

                    if (visuals != null)
                        GetComponent<SpriteRenderer>().sprite = visuals.GetVisual(LevelManager.gridLevel.GetTile(x, y));

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
        Direction dirCurrentTile = LevelManager.gridLevel.GetTile(x, y).ToDirection();
        if (dir != Direction.NULL)
        {
            if (dirCurrentTile == Direction.All || dirCurrentTile == dir)
            {
                dirCurrent = dir;
                dirCurrent.ToOffset(out int offsetX, out int offsetY);

                MoveToCell(x + offsetX, y + offsetY, false);
            } else
            {
                Debug.LogWarning($"Character can't exit tile with direction {dirCurrentTile} to {dir}.", gameObject);
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
        if (LevelManager.gridLevel.CellIsValid(x, y))
        {
            TileType currentTileType = LevelManager.gridLevel.GetTile(x, y);
            if (currentTileType != TileType.NULL)
            {
                IsSafe = currentTileType == TileType.Node;
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