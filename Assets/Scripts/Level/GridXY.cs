using System;
using UnityEngine;

public class GridXY<T>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public Vector3Int position;
    }

    public int height { get; private set; }
    public int width { get; private set; }
    private float cellSize;
    private Vector3 originPosition;
    private T[,] tiles;

    public GridXY(int width, int height, float cellSize, Vector3 originPosition, T defaultGridObjectValue)
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
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            tiles[x, z] = defaultGridObjectValue;
                        }
                    }
                }
            }
        }
    }

    public T GetGridObject(Vector3Int cell)
    {
        T result = default(T);
        if (CellIsValid(cell))
        {
            result = tiles[cell.x, cell.z];
        }
        return result;
    }
    public T GetGridObject(int cellNum)
    {
        return GetGridObject(CellNumToCell(cellNum));
    }

    public void SetGridObject(Vector3Int cell, T value)
    {
        if (CellIsValid(cell))
        {
            Debug.Log($"Setting GridObject {cell.x},{cell.z} {value}.");
            tiles[cell.x, cell.z] = value;
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { position = cell });
        }
    }
    public void SetGridObject(int cellNum, T value)
    {
        SetGridObject(CellNumToCell(cellNum), value);
    }

    public bool CellIsValid(Vector3Int cell)
    {
        return cell.x == Mathf.Clamp(cell.x, 0, width - 1) && cell.z == Mathf.Clamp(cell.z, 0, height - 1);
    }

    public Vector3Int CellNumToCell(int cellNum)
    {
        return new Vector3Int(cellNum % width, 0, cellNum / width);
    }

    public Vector3Int GetCell(Vector3Int startCell, Direction direction)
    {
        return startCell + direction.ToOffset();
    }

}