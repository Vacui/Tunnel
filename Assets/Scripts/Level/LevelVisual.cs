using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    [DisallowMultipleComponent]
    public class LevelVisual : MonoBehaviour
    {
        public static LevelVisual main;

        [SerializeField] private ElementsVisuals skinTiles;

        public Tilemap tilemap;

        [Header("Debug")]
        [SerializeField] bool showDebugLog;

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);

            LevelManager.main.grid.OnGridCreated += (sender, args) => tilemap.ClearAllTiles();
            LevelManager.main.grid.OnGridObjectChanged += (sender, args) => UpdateVisual(args.x, args.y);
        }

        public void UpdateVisual(int x, int y)
        {
            TileType typeTile = LevelManager.main.grid.GetTile(x, y);
            if (showDebugLog) Debug.Log($"Updating Tile {x},{y} ({typeTile}) Visual");
            tilemap.SetTile(new Vector3Int(x, y, 0), skinTiles.GetTileBase(typeTile));
        }

        public void SetTileVisual(int x, int y, Sprite sprite, Color color)
        {
            // TO DELETE
        }

        public void ResetTileVisual(int x, int y, Color color) { }
    }
}