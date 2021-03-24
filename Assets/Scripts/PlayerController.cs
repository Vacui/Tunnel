using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event EventHandler<OnPlayerStartedMoveEventArgs> OnPlayerStartedMove;
    public class OnPlayerStartedMoveEventArgs : EventArgs
    {
        public int x, y;
    }

    [SerializeField, Disable] private int x = -1;
    [SerializeField, Disable] private int y = -1;
    [SerializeField, Disable] private bool isSafe = true;

    [Header("Movement")]
    private const float moveSpeed = 0.1f;
    private const float stretchSpeed = 0.05f;
    private const float shortStretch = 0.7f;
    private Direction dirCurrent = Direction.NULL;
    private int currentScaleTweenId;
    private int currentMoveTweenID;

    private void Update()
    {
        if (isSafe)
        {
            if (Input.GetKeyDown(KeyCode.W)) MoveToCell(Direction.Up);
            else if (Input.GetKeyDown(KeyCode.D)) MoveToCell(Direction.Right);
            else if (Input.GetKeyDown(KeyCode.S)) MoveToCell(Direction.Down);
            else if (Input.GetKeyDown(KeyCode.A)) MoveToCell(Direction.Left);
        }
    }

    public void MoveToCell(int x, int y, bool teleport)
    {
        if (LevelManager.gridLevel.CellIsValid(x, y))
        {
            Debug.Log($"Character moving to cell {x},{y}", gameObject);

            isSafe = false;
            Vector2 nextPos = LevelManager.gridLevel.CellToWorld(x, y);

            this.x = x;
            this.y = y;

            CancelTween(currentMoveTweenID);
            if (teleport)
            {
                CancelMovement();
                transform.position = nextPos;
                CheckCurrentTile();
            } else
                currentMoveTweenID = LeanTween.move(gameObject, nextPos, moveSpeed).setOnComplete(() => CheckCurrentTile()).id;

            OnPlayerStartedMove?.Invoke(this, new OnPlayerStartedMoveEventArgs { x = x, y = y });
        } else
        {
            Debug.LogWarning($"Character can't move to non valid cell {x},{y}.");
            isSafe = true;
        }
    }

    private void MoveToCell(Direction dir)
    {
        Direction dirCurrentTile = LevelManager.gridLevel.GetTile(x, y).ToDirection();
        if (dir != Direction.NULL)
        {
            if (dirCurrentTile == Direction.NULL || dirCurrentTile == dir)
            {
                dirCurrent = dir;
                dirCurrent.ToOffset(out int offsetX, out int offsetY);

                CancelTween(currentScaleTweenId);
                Vector3 tweenScale = new Vector3(shortStretch, shortStretch, 1);
                if (gameObject.transform.localScale != tweenScale)
                    currentScaleTweenId = LeanTween.scale(gameObject, tweenScale, stretchSpeed).id;

                MoveToCell(x + offsetX, y + offsetY, false);
            } else
            {
                Debug.LogWarning($"Character can't exit tile with direction {dirCurrentTile} to {dir}.");
                if (!isSafe)
                    MoveToCell(dirCurrentTile);
            }
        } else
        {
            Debug.LogWarning($"Character can't move in a null direction.");
            isSafe = true;
        }
    }
    private void MoveToCurrentDirection()
    {
        Debug.Log($"Move To Current Direction {dirCurrent}.");
        MoveToCell(dirCurrent);
    }

    private void CheckCurrentTile()
    {
        Debug.Log("Checking current tile");
        if (LevelManager.gridLevel.CellIsValid(x, y))
        {
            isSafe = LevelManager.gridLevel.GetTile(x, y).ToDirection() == Direction.NULL;
            if (!isSafe)
                MoveToCurrentDirection();
            else
                CancelMovement();
        }
    }

    private void CancelMovement()
    {
        //Debug.Log("Canceling movement.");
        CancelTween(currentMoveTweenID);
        CancelTween(currentScaleTweenId);
        currentScaleTweenId = LeanTween.scale(gameObject, Vector2.one, stretchSpeed).id;
    }

    private void CancelTween(int id)
    {
        //Debug.Log($"Canceling tween {id}.");
        if (id > 0 && LeanTween.isTweening(currentScaleTweenId))
            LeanTween.cancel(id);
    }
}