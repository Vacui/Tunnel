using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCoordsEventArgs : EventArgs
{
    public int x, y;
}

public class GridXY<T> {
    public event EventHandler<TileChangedEventArgs> OnTileChanged;
    public class TileChangedEventArgs : EventArgs {
        public int x, y;
        public T previousValue;
        public T value;
    }
    public event EventHandler<GridCreationEventArgs> OnGridCreated;
    public class GridCreationEventArgs : EventArgs {
        public int width;
        public int height;
    }

    public int Height { get; private set; }
    public int Width { get; private set; }
    public int Size { get { return Width * Height; } }

    public int CellSize { get; private set; }
    public Vector3 OriginPosition { get; private set; }

    private T[,] tiles;
    Func<GridXY<T>, int, int, T> tileConstructor;
    private T nullTileValue;
    private bool updateAlways;

    public GridXY() { }

    private bool CreateGridXY(int width, int height, int cellSize, Vector3 originPosition, bool updateAlways, T nullTileValue) {
        if (width > 0) {
            if (height > 0) {
                Width = width;
                Height = height;
                CellSize = cellSize;
                OriginPosition = originPosition;
                this.nullTileValue = nullTileValue;
                this.updateAlways = updateAlways;
                tiles = new T[width, height];
                return true;
            }
        }
        return false;
    }
    public bool CreateGridXY(int width, int height, int cellSize, Vector3 originPosition, bool updateAlways, T nullTileValue, T initializeValue) {
        if (CreateGridXY(width, height, cellSize, originPosition, updateAlways, nullTileValue)) {
            SetAllTiles(initializeValue);
            OnGridCreated?.Invoke(this, new GridCreationEventArgs { width = width, height = height });
            return true;
        }
        return false;
    }

    public bool CreateGridXY(int width, int height, int cellSize, Vector3 originPosition, bool updateAlways, T nullTileValue, Func<GridXY<T>, int, int, T> tileConstructor) {
        if (CreateGridXY(width, height, cellSize, originPosition, updateAlways, nullTileValue)) {
            SetAllTiles(tileConstructor);
            OnGridCreated?.Invoke(this, new GridCreationEventArgs { width = width, height = height });
            return true;
        }
        return false;
    }

    public void SetAllTiles(T value) {
        if (tiles != null)
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    SetTile(x, y, value);
    }

    public void SetAllTiles(Func<GridXY<T>, int, int, T> tileConstructor) {
        if (tiles != null)
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    SetTile(x, y, tileConstructor(this, x, y));
    }

    public T GetTile(int x, int y) {
        if (CellIsValid(x, y))
            return tiles[x, y];
        else
            return nullTileValue;
    }
    public T GetTile(Vector2Int cell) {
        return GetTile(cell.x, cell.y);
    }

    public T GetTile(Vector3 worldPosition) {
        WorldToCell(worldPosition, out int x, out int y);
        return GetTile(x, y);
    }

    public void SetTile(int x, int y, T value) {
        if (CellIsValid(x, y) && (!EqualityComparer<T>.Default.Equals(tiles[x, y], value) || updateAlways)) {
            T previousValue = tiles[x, y];
            tiles[x, y] = value;
            OnTileChanged?.Invoke(this, new TileChangedEventArgs { x = x, y = y, value = value, previousValue = previousValue });
        }
    }
    public void SetTile(Vector2Int cell, T value) {
        SetTile(cell.x, cell.y, value);
    }

    public bool CellIsValid(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
    public bool CellIsValid(Vector2Int cell) {
        return CellIsValid(cell.x, cell.y);
    }

    public void CellNumToCell(int cellNum, out int x, out int y) {
        x = cellNum % Width;
        y = cellNum / Width;
    }

    public Vector3 CellToWorld(int x, int y) {
        return new Vector3(x, y) * CellSize + OriginPosition;
    }

    public void WorldToCell(Vector3 worldPosition, out int x, out int y) {
        worldPosition -= OriginPosition;
        x = Mathf.FloorToInt(worldPosition.x / CellSize);
        y = Mathf.FloorToInt(worldPosition.y / CellSize);
    }

    public List<Vector2Int> GatherNeighbourCells(int x = 0, int y = 0, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        if (radius > 0)
            for (int xT = -radius; xT < radius + 1; xT++)
                for (int yT = -radius; yT < radius + 1; yT++)
                    if (CellIsValid(x + xT, y + yT))
                        if (!avoidCenter || (xT != 0 || yT != 0))
                            if (!avoidCorners || (Mathf.Abs(xT) != Mathf.Abs(yT)))
                                neighbours.Add(new Vector2Int(x + xT, y + yT));
        return neighbours;
    }

    public List<T> GatherNeighbourTiles(int x = 0, int y = 0, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        List<T> neighbours = new List<T>();
        foreach (Vector2Int neighbour in GatherNeighbourCells(x, y, radius, avoidCenter, avoidCorners))
            neighbours.Add(GetTile(neighbour.x, neighbour.y));
        return neighbours;
    }
    public List<T> GatherNeighbourTiles(Vector2Int cell, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        return GatherNeighbourTiles(cell.x, cell.y, radius, avoidCenter, avoidCorners);
    }

    public string GetTileToString(int x, int y) {
        string result = "";

        if (CellIsValid(x, y))
            result = $"{x},{y} ({GetTile(x, y)})";

        return result;
    }
}