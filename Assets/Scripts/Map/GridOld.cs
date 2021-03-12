/* Some of the methods are based on Code Monkey code: https://www.youtube.com/watch?v=waEsGu--9P8 */

using System;
using UnityEngine;

public class GridCoordEventArgs : System.EventArgs {
    public int x;
    public int z;
}

public class GridOld {

    //public event EventHandler<GridCoordEventArgs> OnGridObjectChanged;

    //public int width { get; private set; }
    //public int height { get; private set; }
    //private float cellSize;
    //private Vector3 originPosition;
    //private Tile[,] tilesArray;

    //public GridXZ(int width, int height, float cellSize, Vector3 originPosition) {

    //    if (cellSize <= 0) cellSize = 1;

    //    this.width = width;
    //    this.height = height;
    //    this.originPosition = originPosition;
    //    this.cellSize = cellSize;

    //    tilesArray = new Tile[width, height];

    //    bool showDebug = false;
    //    if (showDebug) {
    //        TextMesh[,] debugTextMeshArray = new TextMesh[width, height];

    //        for (int x = 0; x < tilesArray.GetLength(0); x++) {
    //            for (int z = 0; z < tilesArray.GetLength(1); z++) {
    //                debugTextMeshArray[x, z] = UIUtils.CreateWorldText(tilesArray[x, z]?.ToString(), null, GetCenterWorldPosition(x, z), Quaternion.Euler(90, 0, 0), Color.white, 20, TextAnchor.MiddleCenter);
    //                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
    //                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
    //            }
    //        }
    //        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
    //        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

    //        OnGridObjectChanged += (object sender, GridCoordEventArgs eventArgs) => { debugTextMeshArray[eventArgs.x, eventArgs.z].text = tilesArray[eventArgs.x, eventArgs.z]?.ToString(); };
    //    }

    //}

    //public Vector3 GetWorldPosition(int x, int z) {
    //    return new Vector3(x, 0, z) * cellSize + originPosition;
    //}

    //public Vector3 GetCenterWorldPosition(int x, int z) {
    //    return GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f;
    //}

    //public bool XZValid(int x, int z) {
    //    return x == Mathf.Clamp(x, 0, width - 1) && z == Mathf.Clamp(z, 0, height - 1);
    //}

    //public void GetXZ(Vector3 worldPosition, out int x, out int z) {
    //    x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
    //    z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    //}

    //public void GetXZ(int cellNum, out int x, out int z) {
    //    x = cellNum % width;
    //    z = (cellNum - x) / width;
    //}

    //public void SetTile(int x, int z, Tile tile) {
    //    if (XZValid(x, z) && tile != null) {
    //        tilesArray[x, z] = tile;
    //        TriggerGridObjectChanged(x, z);
    //    } else {
    //        Debug.LogError($"Cell coordinates {x},{z} are not valid.");
    //    }
    //}

    //public void SetTile(Vector3 worldPosition, Tile tile) {
    //    GetXZ(worldPosition, out int x, out int z);
    //    SetTile(x, z, tile);
    //}

    //public void SetTile(int tileNum, Tile tile) {
    //    GetXZ(tileNum, out int x, out int z);
    //    SetTile(x, z, tile);
    //}

    //public void ClearTile(int x, int z)
    //{
    //    if (tilesArray[x, z] != null) { UnityEngine.Object.Destroy(tilesArray[x, z].gameObject); }
    //}
    //public void ClearTile(int tileNum)
    //{
    //    GetXZ(tileNum, out int x, out int z);
    //    ClearTile(x, z);
    //}
    //public void ClearTile(Vector3 worldPosition)
    //{
    //    GetXZ(worldPosition, out int x, out int z);
    //    ClearTile(x, z);
    //}

    //public void TriggerGridObjectChanged(int x, int z) {
    //    OnGridObjectChanged?.Invoke(this, new GridCoordEventArgs { x = x, z = z });
    //}

    //public Tile GetTile(int x, int z) {
    //    if (XZValid(x, z)) {
    //        return tilesArray[x, z];
    //    } else {
    //        return default;
    //    }
    //}

    //public Tile GetTile(Vector3 worldPosition) {
    //    GetXZ(worldPosition, out int x, out int z);
    //    return GetTile(x, z);
    //}

    //public Tile GetTile(int cellNum) {
    //    GetXZ(cellNum, out int x, out int z);
    //    return GetTile(x, z);
    //}

}