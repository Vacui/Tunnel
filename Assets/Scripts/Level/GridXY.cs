using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCoordsEventArgs : EventArgs
{
    public int x, y;
}

public class GridXY<T>
{
    public event EventHandler<TileChangedEventArgs> OnTileChanged;
    public class TileChangedEventArgs : EventArgs
    {
        public int x, y;
        public T value;
    }
    public event EventHandler<GridCreationEventArgs> OnGridCreated;
    public class GridCreationEventArgs : EventArgs
    {
        public int width;
        public int height;
    }

    public int height { get; private set; }
    public int width { get; private set; }
    private T[,] tiles;
    private T nullTileValue;
    private bool updateAlways;

    public GridXY() { }

    public void CreateGridXY(int width, int height, bool updateAlways = false, T initializeValue = default, T nullTileValue = default)
    {
        if (width > 0)
        {
            if (height > 0)
            {
                this.width = width;
                this.height = height;
                this.nullTileValue = nullTileValue;
                this.updateAlways = updateAlways;
                tiles = new T[width, height];
                OnGridCreated?.Invoke(this, new GridCreationEventArgs { width = width, height = height });
                SetAllTiles(initializeValue);
            }
        }
    }

    public void SetAllTiles(T value)
    {
        if (tiles != null)
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    SetTile(x, y, value);
    }

    public T GetTile(int x, int y)
    {
        if (CellIsValid(x, y))
            return tiles[x, y];
        else
            return nullTileValue;
    }

    public void SetTile(int x, int y, T value)
    {
        if (CellIsValid(x, y) && (!EqualityComparer<T>.Default.Equals(tiles[x, y], value) || updateAlways))
        {
            tiles[x, y] = value;
            OnTileChanged?.Invoke(this, new TileChangedEventArgs { x = x, y = y, value = value });
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

    public void WorldToCell(Vector2 world, out int x, out int y)
    {
        throw new NotImplementedException();
    }

    public string GetTileToString(int x, int y)
    {
        string result = "";

        if (CellIsValid(x, y))
            result = $"{x},{y} ({GetTile(x, y)})";

        return result;
    }
}