using System;
using UnityEngine;

public class GridCoordsEventArgs : EventArgs
{
    public int x, y;
}

public class GridCreationEventArgs : EventArgs
{
    public int width;
    public int height;
    public float cellSize;
    public Vector2 originPosition;
}

public class GridXY<T>
{
    public event EventHandler<GridObjectChangedEventArgs> OnGridObjectChanged;
    public class GridObjectChangedEventArgs : EventArgs
    {
        public int x, y;
        public T value;
    }
    public event EventHandler<GridCreationEventArgs> OnGridCreated;

    public int height { get; private set; }
    public int width { get; private set; }
    private float cellSize;
    private Vector2 originPosition;
    private T[,] tiles;
    private T defaultTileValue;

    public GridXY() { }

    public void CreateGridXY(int width, int height, float cellSize, Vector3 originPosition)
    {
        if (width > 0)
        {
            if (height > 0)
            {
                if (cellSize > 0)
                {
                    this.width = width;
                    this.height = height;
                    this.cellSize = cellSize;
                    this.originPosition = originPosition;
                    tiles = new T[width, height];
                    OnGridCreated?.Invoke(this, new GridCreationEventArgs { width = width, height = height, cellSize = cellSize, originPosition = originPosition });
                }
            }
        }
    }

    public void SetAllTiles(T value)
    {
        if (tiles != null)
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    tiles[x, z] = value;
    }

    public T GetTile(int x, int y)
    {
        T result = default(T);
        if (CellIsValid(x, y))
        {
            result = tiles[x, y];
        } else
            throw new NullReferenceException();
        return result;
    }

    public void SetTile(int x, int y, T value)
    {
        if (CellIsValid(x, y))
        {
            tiles[x, y] = value;
            OnGridObjectChanged?.Invoke(this, new GridObjectChangedEventArgs { x = x, y = y, value = value });
        }
    }

    public bool CellIsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void CellNumToCell(int cellNum, out int x, out int y)
    {
        x = cellNum % width;
        y = cellNum / width;
    }

    public Vector2 CellToWorld(int x, int y)
    {
        return new Vector2(x, y) * new Vector2(1,-1) * cellSize + originPosition;
    }

    public void WorldToCell(Vector2 world, out int x, out int y)
    {
        x = Mathf.RoundToInt((world.x - originPosition.x) / (1 * cellSize));
        y = Mathf.RoundToInt((world.y - originPosition.y) / (-1 * cellSize));
        if (!CellIsValid(x, y)) x = y = -1;
    }

    public string GetTileToString(int x, int y)
    {
        string result = "";

        if (CellIsValid(x, y))
            result = $"{x},{y} ({GetTile(x, y)})";

        return result;
    }
}