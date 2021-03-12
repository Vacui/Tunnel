using UnityEngine;

namespace Tunnel
{
    public class GridXZ
    {
        private int height;
        private int width;
        private float cellSize;
        private Vector3 originPosition;
        private Tile[,] tiles;

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

                        tiles = new Tile[width, height];
                    }
                }
            }
        }

        public Tile GetTile(Vector3Int cell)
        {
            Tile result = null;
            if(CellIsValid(cell))
            {
                result = tiles[cell.x, cell.z];
            }
            return result;
        }

        public void SetTile(int cellNum, Tile tile)
        {
            Vector3Int cell = CellNumToCell(cellNum);
            if (CellIsValid(cell))
            {
                Debug.Log($"Setting tile {cell.x},{cell.z} {tile.GetTileType()}.");
                tiles[cell.x, cell.z] = tile;
                tile.RefreshTile(new Vector3Int(cell.x, 0, cell.z), this);
                if (CellIsValid(cell + new Vector3Int(0, 0, 1))) if (tiles[cell.x, cell.z + 1] != null) tiles[cell.x, cell.z + 1].RefreshTile(cell + new Vector3Int(0, 0, 1), this);
                if (CellIsValid(cell + new Vector3Int(1, 0, 0))) if (tiles[cell.x + 1, cell.z] != null) tiles[cell.x + 1, cell.z].RefreshTile(cell + new Vector3Int(1, 0, 0), this);
                if (CellIsValid(cell + new Vector3Int(0, 0, -1))) if (tiles[cell.x, cell.z - 1] != null) tiles[cell.x, cell.z - 1].RefreshTile(cell + new Vector3Int(0, 0, -1), this);
                if (CellIsValid(cell + new Vector3Int(-1, 0, 0))) if (tiles[cell.x - 1, cell.z] != null) tiles[cell.x - 1, cell.z].RefreshTile(cell + new Vector3Int(-1, 0, 0), this);
            }
        }

        public bool CellIsValid(Vector3Int cell)
        {
            return cell.x == Mathf.Clamp(cell.x, 0, width - 1) && cell.z == Mathf.Clamp(cell.z, 0, height - 1);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return new Vector3(cell.x, 0, cell.z) * cellSize + originPosition;
        }

        private Vector3Int CellNumToCell(int cellNum)
        {
            return new Vector3Int(cellNum % width, 0, cellNum / width);
        }

        public Vector3Int GetCell(Vector3Int startCell, Direction direction)
        {
            return startCell + direction.ToOffset();
        }

    }
}