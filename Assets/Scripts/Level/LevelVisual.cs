using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level {
    [DisallowMultipleComponent, RequireComponent(typeof(Tilemap))]
    public class LevelVisual : MonoBehaviour {
        public static LevelVisual main { get; private set; }

        [SerializeField] private TileBase t_start;
        [SerializeField] private TileBase t_end;
        [SerializeField] private TileBase t_node;
        [SerializeField] private TileBase t_tunnel;

        public Tilemap Tilemap { get; private set; }

        [System.Flags]
        enum LevelVisualDebug {
            Nothing = 0,
            Hiding_Tile = 1,
            Discovering_Tile = 2,
            Updating_Visual = 4,
            Everything = ~0
        }
        [Header("Debug")]
        [SerializeField, EnumFlag] private LevelVisualDebug showDebugLog;

        private void Awake() {
            if (main == null) main = this;
            else Destroy(this);

            Tilemap = GetComponent<Tilemap>();

            LevelManager.Main.Grid.OnGridCreated += (sender, args) => {
                Debug.Log("Clearing visual Tilemap");
                Tilemap.ClearAllTiles();
            };
            //LevelManager.main.Grid.OnTileChanged += (sender, args) => {
            //    UpdateVisual(args.x, args.y);
            //};

            LevelFog.HiddenTile += (sender, args) => {
                if (showDebugLog.HasFlag(LevelVisualDebug.Hiding_Tile)) Debug.Log($"Hiding Visual Tile {args.x},{args.y}");
                Tilemap.SetTile(new Vector3Int(args.x, args.y, 0), null);
            };
            LevelFog.DiscoveredTile += (sender, args) => {
                if (showDebugLog.HasFlag(LevelVisualDebug.Discovering_Tile)) Debug.Log($"Discovering Visual Tile {args.x},{args.y}");
                UpdateVisual(args.x, args.y);
            };
        }

        public void UpdateVisual(int x, int y) {
            if (LevelManager.Main.LvlState != LevelManager.LevelState.Win) {
                Element element = LevelManager.Main.Grid.GetTile(x, y);
                if (showDebugLog.HasFlag(LevelVisualDebug.Updating_Visual)) Debug.Log($"Updating Visual Tile {x},{y} ({element})");
                Tilemap.SetTile(new Vector3Int(x, y, 0), GetTileBase(element));
            }
        }

        private TileBase GetTileBase(Element type) {
            switch (type) {
                case Element.NULL: return null;
                case Element.Start: return t_start;
                case Element.End: return t_end;
                case Element.Node: return t_node;
                default: return t_tunnel;
            }
        }

        private void OnDisable() {
            Tilemap.ClearAllTiles();
        }
    }
}