using System;
using UnityEngine;

public class GridXY<T>
{
    public event EventHandler<OnTileChangedEventArgs> OnTileChanged;
    public class OnTileChangedEventArgs : EventArgs
    {
        public int x, y;
    }

    public int height { get; private set; }
    public int width { get; private set; }
    private float cellSize;
    private Vector2 originPosition;
    private T[,] tiles;
    private T defaultTileValue;

    public GridXY(int width, int height, float cellSize, Vector2 originPosition, T defaultTileValue)
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
                } else Debug.LogWarning($"Can't create a grid with cellSize {cellSize}.");
            } else Debug.LogWarning($"Can't create a grid with height {height}.");
        } else Debug.LogWarning($"Can't create a grid with width {width}.");
    }

    public void SetTile(int x, int y, T value)
    {
        if (CellIsValid(x, y))
        {
            tiles[x, y] = value;
            OnTileChanged?.Invoke(this, new OnTileChangedEventArgs { x = x, y = y });
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
    }
    public Vector2Int CellNumToCell(int cellNum)
    {
        CellNumToCell(cellNum, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public Vector2 CellToWorld(int x, int y)
    {
        return new Vector2(x, y) * cellSize + originPosition;
    }

    public void ClearAllTiles()
    {
        if (tiles != null)
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    tiles[x, z] = defaultTileValue;
    }
}