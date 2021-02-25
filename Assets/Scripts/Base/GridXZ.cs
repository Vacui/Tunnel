/* Some of the methods are based on Code Monkey code: https://www.youtube.com/watch?v=waEsGu--9P8 */

using System;
using UnityEngine;

public class GridXZ<TGridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int z;
    }

    public int width { get; private set; }
    public int height { get; private set; }
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    public GridXZ(int width, int height, float cellSize) : this(width, height, cellSize, Vector2.zero) { }
    public GridXZ(int width, int height, float cellSize, Vector2 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject = null) {

        if (cellSize <= 0) cellSize = 1;

        this.width = width;
        this.height = height;
        this.originPosition = new Vector3(originPosition.x, 0, originPosition.y);
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebug = false;
        if (showDebug) {
            TextMesh[,] debugTextMeshArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    debugTextMeshArray[x, z] = UIUtils.CreateWorldText(gridArray[x, z]?.ToString(), null, GetCenterWorldPosition(x, z), Quaternion.Euler(90, 0, 0), Color.white, 20, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => { debugTextMeshArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString(); };
        }

    }

    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public Vector3 GetCenterWorldPosition(int x, int z) {
        return GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f;
    }

    public bool XZValid(int x, int z) {
        return x == Mathf.Clamp(x, 0, width - 1) && z == Mathf.Clamp(z, 0, height - 1);
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void GetXZ(int cellNum, out int x, out int z) {
        x = cellNum % width;
        z = (cellNum - x) / width;
    }

    public void SetGridObject(int x, int z, TGridObject value) {
        if (XZValid(x, z)) {
            gridArray[x, z] = value;
            TriggerGridObjectChanged(x, z);
        } else {
            Debug.LogError($"Cell coordinates {x},{z} are not valid.");
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public void SetGridObject(int cellNum, TGridObject value) {
        GetXZ(cellNum, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public void TriggerGridObjectChanged(int x, int z) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }

    public TGridObject GetGridObject(int x, int z) {
        if (XZValid(x, z)) {
            return gridArray[x, z];
        } else {
            return default;
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        GetXZ(worldPosition, out int x, out int z);
        return GetGridObject(x, z);
    }

    public TGridObject GetGridObject(int cellNum) {
        GetXZ(cellNum, out int x, out int z);
        return GetGridObject(x, z);
    }

}