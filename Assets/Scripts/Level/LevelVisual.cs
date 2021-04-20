using UnityEngine;

namespace Level
{
    [DisallowMultipleComponent]
    public class LevelVisual : MonoBehaviour
    {
        public static LevelVisual main;

        public GridXY<SpriteRenderer> grid { get; private set; }
        [SerializeField] private ElementsVisuals skinTiles;

        private Transform transformLevel;

        [Header("Debug")]
        [SerializeField] bool showDebugLog;

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);

            grid = new GridXY<SpriteRenderer>();
            LevelManager.main.grid.OnGridCreated += (object sender, GridCreationEventArgs args) =>
            {
                ResetVisuals();
                grid.CreateGridXY(args.width, args.height, args.cellSize, args.originPosition);
            };
            LevelManager.main.grid.OnGridObjectChanged += (object sender, GridXY<TileType>.GridObjectChangedEventArgs args) => ResetTileVisual(args.x, args.y);
        }

        /// <summary>
        /// Clear the game visuals. USE WITH CAUTION!
        /// </summary>
        private void ResetVisuals()
        {
            if (showDebugLog) Debug.Log("Deleting visuals");
            if (transformLevel)
                Destroy(transformLevel.gameObject);

            transformLevel = new GameObject("level").transform;
            transformLevel.parent = transform;
            transformLevel.localPosition = Vector3.zero;
            transformLevel.localRotation = Quaternion.identity;
        }

        public void ResetTileVisual(int x, int y, Color color)
        {
            TileType typeTile = LevelManager.main.grid.GetTile(x, y);
            if (showDebugLog) Debug.Log($"Updating Tile {x},{y} ({typeTile}) Visual");
            SetTileVisual(x, y, skinTiles.GetVisualData(typeTile));
        }
        public void ResetTileVisual(int x, int y) { ResetTileVisual(x, y, Color.black); }

        public void SetTileVisual(int x, int y, Sprite sprite, Color color)
        {
            if (grid.CellIsValid(x, y) && sprite)
            {
                SpriteRenderer visualTile = grid.GetTile(x, y);
                if (!visualTile)
                {
                    visualTile = SpawnVisuals(x, y);
                    grid.SetTile(x, y, visualTile);
                }

                visualTile.sprite = sprite;
                visualTile.color = color;
            }
        }
        public void SetTileVisual(int x, int y, Sprite sprite) { SetTileVisual(x, y, sprite, Color.black); }
        public void SetTileVisual(int x, int y, ElementsVisuals.VisualData visualData) { SetTileVisual(x, y, visualData.sprite, visualData.color); }

        private SpriteRenderer SpawnVisuals(int x, int y)
        {
            SpriteRenderer newTile = new GameObject($"{x},{y}").AddComponent<SpriteRenderer>();
            newTile.color = Color.black;
            newTile.transform.parent = transformLevel;
            newTile.transform.localPosition = grid.CellToWorld(x, y);
            return newTile;
        }
    }
}