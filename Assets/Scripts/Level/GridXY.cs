using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCellEventArgs : EventArgs {
    public int x, y;
    public Vector2Int cell;
}

public class GridXY<T> {
    /// <summary>
    /// Event called when a tile in the grid is changed, every time if the parameter <see cref="updateAlways"/> is true.
    /// </summary>
    public event EventHandler<TileChangedEventArgs> OnTileChanged;
    public class TileChangedEventArgs : EventArgs {
        public int x, y;
        public T previousValue;
        public T value;
    }

    /// <summary>
    /// Event called each time the grid is created.
    /// </summary>
    public event EventHandler<GridCreationEventArgs> OnGridCreated;
    public class GridCreationEventArgs : EventArgs {
        public int width;
        public int height;
        public int cellSize;
    }

    /// <summary>
    /// Grid height in number of cells.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Grid width in number of cells.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Grid size in number of cells (width * height).
    /// </summary>
    public int Size { get { return Width * Height; } }

    /// <summary>
    /// Size of a single cell.
    /// </summary>
    public int CellSize { get; private set; }

    /// <summary>
    /// Grid origin position, used to convert cells in world position
    /// </summary>
    public Vector3 OriginPosition { get; private set; }

    private T[,] tiles;
    Func<GridXY<T>, int, int, T> tileConstructor;
    private T nullTileValue;
    private bool updateAlways;

    /// <summary>
    /// Constructor. Use one of the CreateGridXT methods to create the grid.
    /// </summary>
    public GridXY() { }

    /// <summary>
    /// Create the grid.
    /// </summary>
    /// <param name="width">Width in cells.</param>
    /// <param name="height">Height in cells.</param>
    /// <param name="cellSize">Cell size.</param>
    /// <param name="originPosition">Grid origin position.</param>
    /// <param name="updateAlways">Call the event <see cref="OnTileChanged"/> at each tile change.</param>
    /// <param name="nullTileValue">Value to return if a cell is not valid.</param>
    /// <returns>False if a value is not valid. True otherwise.</returns>
    private bool CreateGridXY(int width, int height, int cellSize, Vector3 originPosition, bool updateAlways, T nullTileValue) {

        if(width <= 0) {
            return false;
        }

        if(height <= 0) {
            return false;
        }

        Width = width;
        Height = height;
        CellSize = cellSize;
        OriginPosition = originPosition;
        this.nullTileValue = nullTileValue;
        this.updateAlways = updateAlways;
        tiles = new T[width, height];
        return true;
    }

    /// <summary>
    /// Create the grid.
    /// </summary>
    /// <param name="width">Width in cells.</param>
    /// <param name="height">Height in cells.</param>
    /// <param name="cellSize">Cell size.</param>
    /// <param name="originPosition">Grid origin position.</param>
    /// <param name="updateAlways">Call the event <see cref="OnTileChanged"/> at each tile change.</param>
    /// <param name="nullTileValue">Value to return if a cell is not valid.</param>
    /// <param name="initializeValue">Value to initialize all the tiles.</param>
    /// <returns>False if a value is not valid. True otherwise.</returns>
    public bool CreateGridXY(int width, int height, int cellSize, Vector3 originPosition, bool updateAlways, T nullTileValue, T initializeValue) {
        
        if(!CreateGridXY(width, height, cellSize, originPosition, updateAlways, nullTileValue)) {
            return false;
        }

        SetAllTiles(initializeValue);
        OnGridCreated?.Invoke(this, new GridCreationEventArgs { width = width, height = height, cellSize = cellSize });
        return true;
    }

    /// <summary>
    /// Create the grid.
    /// </summary>
    /// <param name="width">Width in cells.</param>
    /// <param name="height">Height in cells.</param>
    /// <param name="cellSize">Cell size.</param>
    /// <param name="originPosition">Grid origin position.</param>
    /// <param name="updateAlways">Call the event <see cref="OnTileChanged"/> at each tile change.</param>
    /// <param name="nullTileValue">Value to return if a cell is not valid.</param>
    /// <param name="tileConstructor">Function to call when creating a tile.</param>
    /// <returns>False if a value is not valid. True otherwise.</returns>
    public bool CreateGridXY(int width, int height, int cellSize, Vector3 originPosition, bool updateAlways, T nullTileValue, Func<GridXY<T>, int, int, T> tileConstructor) {

        if (!CreateGridXY(width, height, cellSize, originPosition, updateAlways, nullTileValue)) {
            return false;
        }
            SetAllTiles(tileConstructor);
            OnGridCreated?.Invoke(this, new GridCreationEventArgs { width = width, height = height });
        return true;
    }

    /// <summary>
    /// Set all tiles to a specified value.
    /// </summary>
    /// <param name="value">Tiles' new value</param>
    public void SetAllTiles(T value) {
        if(tiles == null) {
            return;
        }

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                SetTile(x, y, value);
            }
        }
    }

    /// <summary>
    /// Set all tiles value using the specified constructor function.
    /// </summary>
    /// <param name="value">Tiles' new value</param>
    public void SetAllTiles(Func<GridXY<T>, int, int, T> tileConstructor) {
        if(tiles == null) {
            return;
        }

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                SetTile(x, y, tileConstructor(this, x, y));
            }
        }
    }

    /// <summary>
    /// Get a tile.
    /// </summary>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    /// <returns>Tile value or <see cref="nullTileValue"/> if the parameters are not valid.</returns>
    public T GetTile(int x, int y) {
        if(!CellIsValid(x, y)) {
            return nullTileValue;
        }

        return tiles[x, y];
    }

    /// <summary>
    /// Get a tile.
    /// </summary>
    /// <param name="cell">Tile cell.</param>
    /// <returns>Tile value or <see cref="nullTileValue"/> if the parameters are not valid.</returns>
    public T GetTile(Vector2Int cell) {
        return GetTile(cell.x, cell.y);
    }

    /// <summary>
    /// Get a tile.
    /// </summary>
    /// <param name="worldPosition">Tile world position.</param>
    /// <returns>Tile value or <see cref="nullTileValue"/> if the parameters are not valid.</returns>
    public T GetTile(Vector3 worldPosition) {
        WorldToCell(worldPosition, out int x, out int y);
        return GetTile(x, y);
    }

    /// <summary>
    /// Set a tile.
    /// </summary>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    /// <param name="value">New Tile value.</param>
    public void SetTile(int x, int y, T value) {
        if(!CellIsValid(x, y)) {
            return;
        }

        if(!updateAlways && EqualityComparer<T>.Default.Equals(tiles[x, y], value)) {
            return;
        }

        T previousValue = tiles[x, y];
        tiles[x, y] = value;
        OnTileChanged?.Invoke(this, new TileChangedEventArgs { x = x, y = y, value = value, previousValue = previousValue });
    }

    /// <summary>
    /// Set a tile.
    /// </summary>
    /// <param name="cell">Tile cell.</param>
    /// <param name="value">New Tile value.</param>
    public void SetTile(Vector2Int cell, T value) {
        SetTile(cell.x, cell.y, value);
    }

    /// <summary>
    /// Set a tile.
    /// </summary>
    /// <param name="cellNum">Tile num.</param>
    /// <param name="value">New Tile value.</param>
    public void SetTile(int cellNum, T value) {
        CellNumToCell(cellNum, out int x, out int y);
        SetTile(x, y, value);
    }

    /// <summary>
    /// Check if a cell is valid in the grid.
    /// </summary>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    /// <returns>Returns true if the cell is valid, false if otherwise.</returns>
    public bool CellIsValid(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    /// <summary>
    /// Check if a cell is valid in the grid.
    /// </summary>
    /// <param name="cell">Cell.</param>
    /// <returns>Returns true if the cell is valid, false if otherwise.</returns>
    public bool CellIsValid(Vector2Int cell) {
        return CellIsValid(cell.x, cell.y);
    }

    /// <summary>
    /// Converts a cell num its coordinates.
    /// </summary>
    /// <param name="cellNum">Cell num.</param>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    public void CellNumToCell(int cellNum, out int x, out int y) {
        x = cellNum % Width;
        y = cellNum / Width;
    }

    /// <summary>
    /// Converts a cell into its world position (cell * <see cref="CellSize"/> + <see cref="OriginPosition"/>).
    /// </summary>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    /// <returns>Cell world position.</returns>
    public Vector3 CellToWorld(int x, int y) {
        return new Vector3(x, y) * CellSize + OriginPosition;
    }

    /// <summary>
    /// Converts a world position to its cell coordinates. The result cell may not be valid.
    /// </summary>
    /// <param name="worldPosition">World position.</param>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    public void WorldToCell(Vector3 worldPosition, out int x, out int y) {
        worldPosition -= OriginPosition;
        x = Mathf.FloorToInt(worldPosition.x / CellSize);
        y = Mathf.FloorToInt(worldPosition.y / CellSize);
    }

    /// <summary>
    /// Return all neighburs cells of a cell in a radius.
    /// </summary>
    /// <param name="x">Origin cell X. Default: 0</param>
    /// <param name="y">Origin cell Y. Default: 0</param>
    /// <param name="radius">Research radius, min 1. Default: 1</param>
    /// <param name="avoidCenter">Not insert the origin cell in the result. Default: false</param>
    /// <param name="avoidCorners">Avoid consider diagonals. Default: false</param>
    /// <returns>Null if one of the parameters is not valid. The list of neighburs' cells otherwise.</returns>
    public List<Vector2Int> GatherNeighbourCells(int x = 0, int y = 0, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        if(radius <= 0) {
            return null;
        }

        if(!CellIsValid(x, y)) {
            return null;
        }

        List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int xT = -radius; xT < radius + 1; xT++) {
            for (int yT = -radius; yT < radius + 1; yT++) {
                if (!CellIsValid(x + xT, y + yT)) {
                    continue;
                }

                if(avoidCenter && xT == 0 && yT == 0) {
                    continue;
                }

                if(avoidCorners && (Mathf.Abs(xT) == Mathf.Abs(yT))){
                    continue;
                }

                neighbours.Add(new Vector2Int(x + xT, y + yT));
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Return all neighburs cells of a cell in a radius.
    /// </summary>
    /// <param name="cell">Origin cell.</param>
    /// <param name="radius">Research radius, min 1. Default: 1</param>
    /// <param name="avoidCenter">Not insert the origin cell in the result. Default: false</param>
    /// <param name="avoidCorners">Avoid consider diagonals. Default: false</param>
    /// <returns>Null if one of the parameters is not valid. The list of neighburs' cells otherwise.</returns>
    public List<Vector2Int> GatherNeighbourCells(Vector2Int cell, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        return GatherNeighbourCells(cell.x, cell.y, radius, avoidCenter, avoidCorners);
    }

    /// <summary>
    /// Return all neighburs tiles of a cell in a radius.
    /// </summary>
    /// <param name="x">Origin cell X.</param>
    /// <param name="y">Origin cell Y.</param>
    /// <param name="radius">Research radius, min 1. Default: 1</param>
    /// <param name="avoidCenter">Not insert the origin cell in the result. Default: false</param>
    /// <param name="avoidCorners">Avoid consider diagonals. Default: false</param>
    /// <returns>Null if one of the parameters is not valid. The list of neighburs' tiles otherwise.</returns>
    public List<T> GatherNeighbourTiles(int x = 0, int y = 0, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        List<T> neighbours = new List<T>();
        foreach (Vector2Int neighbour in GatherNeighbourCells(x, y, radius, avoidCenter, avoidCorners)) {
            neighbours.Add(GetTile(neighbour.x, neighbour.y));
        }
        return neighbours;
    }

    /// <summary>
    /// Return all neighburs tiles of a cell in a radius.
    /// </summary>
    /// <param name="cell">Origin cell.</param>
    /// <param name="radius">Research radius, min 1. Default: 1</param>
    /// <param name="avoidCenter">Not insert the origin cell in the result. Default: false</param>
    /// <param name="avoidCorners">Avoid consider diagonals. Default: false</param>
    /// <returns>Null if one of the parameters is not valid. The list of neighburs' tiles otherwise.</returns>
    public List<T> GatherNeighbourTiles(Vector2Int cell, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        return GatherNeighbourTiles(cell.x, cell.y, radius, avoidCenter, avoidCorners);
    }

    /// <summary>
    /// Returns a tile information in this format "x,y tileValue"
    /// </summary>
    /// <param name="x">Tile cell X.</param>
    /// <param name="y">Tile cell Y.</param>
    /// <returns>Tile information. Null if the cell is not valid.</returns>
    public string GetTileToString(int x, int y) {
        if(!CellIsValid(x, y)) {
            return null;
        }

        return $"{x},{y} ({GetTile(x, y)})";
    }
}