using System;
using UnityEngine;

public class GridXZ<T>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public Vector3Int position;
    }

    private int height;
    private int width;
    private float cellSize;
    private Vector3 originPosition;
    private T[,] tiles;

    public GridXZ(int width, int height, float cellSize, Vector3 originPosition)
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

    public void SetGridObject(int cellNum, T value)
    {
        Vector3Int position = CellNumToCell(cellNum);
        if (CellIsValid(position))
        {
            Debug.Log($"Setting tile {position.x},{position.z} {value}.");
            tiles[position.x, position.z] = value;
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { position = position});
        }
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