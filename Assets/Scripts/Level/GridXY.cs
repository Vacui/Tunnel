using System;
using UnityEngine;

public class GridXY<T>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public int height { get; private set; }
    public int width { get; private set; }
    private float cellSize;
    private Vector2 originPosition;
    private T[,] tiles;
    private T defaultTileValue;

    public GridXY(int width, int height, float cellSize, Vector3 originPosition, T defaultTileValue)
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
                    this.defaultTileValue = defaultTileValue;
                    tiles = new T[width, height];
                    ClearAllTiles();
                }
            }
        }
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
    public T GetTile(int cellNum)
    {
        CellNumToCell(cellNum, out int x, out int y);
        return GetTile(x, y);
    }
    public T GetTile(int x, int y, Direction dir)
    {
        Debug.Log($"GetTile {x},{y},{dir}.");
        T result = default(T);
        if(dir != Direction.NULL)
        {
            dir.ToOffset(out int offsetX, out int offsetY);
            Debug.Log($"offset {offsetX},{offsetY}.");
            result = GetTile(x + offsetX, y + offsetY);
        }
        return result;
    }

    public void SetTile(int x, int y, T value)
    {
        if (CellIsValid(x, y))
        {
            tiles[x, y] = value;
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }
    public void SetTile(int cellNum, T value)
    {
        CellNumToCell(cellNum, out int x, out int y);
        SetTile(x, y, value);
    }

    public bool CellIsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void CellNumToCell(int cellNum, out int x, out int y)
    {
        x = cellNum % width;
        y = cellNum / width;
        Debug.Log($"{cellNum} = x={x}, y={y}");
    }
    public Vector2Int CellNumToCell(int cellNum)
    {
        CellNumToCell(cellNum, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public Vector2 CellToWorld(int x, int y)
    {
        return new Vector2(x, y) * new Vector2(1,-1) * cellSize + originPosition;
    }

    public void ClearAllTiles()
    {
        if (tiles != null)
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    tiles[x, z] = defaultTileValue;
    }
}