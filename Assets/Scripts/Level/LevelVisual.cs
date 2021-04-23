using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    [DisallowMultipleComponent, RequireComponent(typeof(Tilemap))]
    public class LevelVisual : MonoBehaviour
    {
        public static LevelVisual main { get; private set; }

        [SerializeField] private TileBase t_start;
        [SerializeField] private TileBase t_end;
        [SerializeField] private TileBase t_node;
        [SerializeField] private TileBase t_tunnel;

        public Tilemap Tilemap { get; private set; }

        [Header("Debug")]
        [SerializeField] bool showDebugLog;

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);

            Tilemap = GetComponent<Tilemap>();

            LevelManager.main.grid.OnGridCreated += (sender, args) => Tilemap.ClearAllTiles();
            LevelManager.main.grid.OnTileChanged += (sender, args) => UpdateVisual(args.x, args.y);
        }

        public void UpdateVisual(int x, int y)
        {
            TileType typeTile = LevelManager.main.grid.GetTile(x, y);
            if (showDebugLog) Debug.Log($"Updating Tile {x},{y} ({typeTile}) Visual");
            Tilemap.SetTile(new Vector3Int(x, y, 0), GetTileBase(typeTile));
        }

        private TileBase GetTileBase(TileType type)
        {
            switch (type)
            {
                case TileType.NULL: return null;
                case TileType.Player: return t_start;
                case TileType.Goal: return t_end;
                case TileType.Node: return t_node;
                default: return t_tunnel;
            }
        }
    }
}