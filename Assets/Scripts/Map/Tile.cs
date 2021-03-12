using System.Collections.Generic;
using UnityEngine;

namespace Tunnel
{
    public class Tile
    {
        public static RuleTileVisual RuleTileVisual;

        private TileType type;
        private GameObject visual;

        public Tile(TileType type)
        {
            this.type = type;
        }

        public void RefreshTile(Vector3Int cell, GridXZ grid)
        {
            Debug.Log($"Refreshing tile {cell.x},{cell.z} {type}.");
            TileType[] neighbours = new TileType[5];
            neighbours[0] = type;

            neighbours[1] = GetNeighbourTileType(cell, grid);
            neighbours[2] = GetNeighbourTileType(cell, grid);
            neighbours[3] = GetNeighbourTileType(cell, grid);
            neighbours[4] = GetNeighbourTileType(cell, grid);

            Debug.Log($"Getting visual {TileTypeUtils.TileTypeArrayToString(neighbours)}.");
            GameObject model = RuleTileVisual.GetVisual(neighbours);

            if(model != null)
            {
                //Debug.Log($"Setting visual at {x},{z}");
                SetVisual(model, grid.CellToWorld(cell));
            }
        }

        private TileType GetNeighbourTileType(Vector3Int cell, GridXZ grid)
        {
            TileType result = TileType.NULL;
            Tile tile = grid.GetTile(cell);
            if (tile != null) result = tile.GetTileType();
            return result;
        }

        public TileType GetTileType() { return type; }

        private void SetVisual(GameObject model, Vector3 worldPosition)
        {
            if(visual != null)
            {
                Object.Destroy(visual);
            }
            visual = Object.Instantiate(model);
            visual.transform.position = worldPosition;
        }
    }
}