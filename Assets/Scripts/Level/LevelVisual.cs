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
            //LevelManager.main.grid.OnTileChanged += (sender, args) => UpdateVisual(args.x, args.y);

            LevelFog.HiddenTile += (sender, args) =>
            {
                if (showDebugLog) Debug.Log($"Hiding Visual Tile {args.x},{args.y}");
                Tilemap.SetTile(new Vector3Int(args.x, args.y, 0), null);
            };
            LevelFog.DiscoveredTile += (sender, args) => UpdateVisual(args.x, args.y);

            LevelPalette.Updated += (color) => Tilemap.color = color;
        }

        public void UpdateVisual(int x, int y)
        {
            Element element = LevelManager.main.grid.GetTile(x, y);
            if (showDebugLog) Debug.Log($"Updating Visual Tile {x},{y} ({element})");
            Tilemap.SetTile(new Vector3Int(x, y, 0), GetTileBase(element));
        }

        private TileBase GetTileBase(Element type)
        {
            switch (type)
            {
                case Element.NULL: return null;
                case Element.Start: return t_start;
                case Element.End: return t_end;
                case Element.Node: return t_node;
                default: return t_tunnel;
            }
        }
    }
}