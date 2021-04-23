using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    [DisallowMultipleComponent, RequireComponent(typeof(Tilemap))]
    public class LevelBackground : MonoBehaviour
    {
        [SerializeField] private TileBase visual = null;

        private Tilemap tilemap;
        private GridXY<bool> grid;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            grid = new GridXY<bool>();

            grid.OnTileChanged += (sender, args) => tilemap.SetTile(new Vector3Int(args.x, args.y, 0), visual);

            LevelManager.main.grid.OnGridCreated += (sender, args) =>
            {
                tilemap.ClearAllTiles();
                grid.CreateGridXY(args.width + 2, args.height + 2, args.cellSize, args.originPosition, false);
            };
        }
    }
}