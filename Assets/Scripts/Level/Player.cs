using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Disable] private int x = -1;
    [SerializeField, Disable] private int y = -1;
    [SerializeField, Disable] private bool isSafe = true;

    [Header("Movement")]
    private Direction dirCurrent = Direction.NULL;

    public Player(int x, int y)
    {
        this.x = x;
        this.y = y;
        MoveToStartCell(x, y);
    }

    private void Update()
    {
        if (isSafe)
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
                Debug.Log($"Character moving to cell {x},{y}");

                isSafe = false;
                Vector2 nextPos = LevelManager.gridLevel.CellToWorld(x, y);

                if (!teleport)
                    LevelManager.Instance.ExitTile(this.x, this.y);

                this.x = x;
                this.y = y;

                LevelManager.Instance.EnterTile(x, y);
                CheckCurrentTile();
            } else
            {
                Debug.LogWarning($"Character can't move to non valid cell {x},{y}.");
                isSafe = true;
            }
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
                MoveToCell(x + offsetX, y + offsetY, false);
            } else
            {
                Debug.LogWarning($"Character can't exit tile with direction {dirCurrentTile} to {dir}.");
                if (!isSafe)
                    MoveToCurrentDirection();
            }
        } else
        {
            Debug.LogWarning($"Character can't move in a null direction.");
            isSafe = true;
        }
    }

    public void MoveToStartCell(int x, int y)
    {
        MoveToCell(x, y, true);
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
        Debug.Log("Canceling movement.");
    }
}