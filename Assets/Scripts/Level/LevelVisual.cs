using UnityEngine;

[DisallowMultipleComponent]
public class LevelVisual : MonoBehaviour
{
    public GridXY<SpriteRenderer> grid { get; private set; }
    [SerializeField] private ElementsVisuals skinTiles;

    private Transform transformLevel;

    [Header("Debug")]
    [SerializeField] bool showDebugLog;

    private void Awake()
    {
        grid = new GridXY<SpriteRenderer>();
        Singletons.main.lvlManager.grid.OnGridCreated += (object sender, GridCreationEventArgs args) =>
        {
            ResetVisuals();
            grid.CreateGridXY(args.width, args.height, args.cellSize, args.originPosition);
        };
        Singletons.main.lvlManager.grid.OnGridObjectChanged += (object sender, GridXY<TileType>.GridObjectChangedEventArgs args) => ResetTileVisual(args.x, args.y);
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
        TileType typeTile = Singletons.main.lvlManager.grid.GetTile(x, y);
        if (showDebugLog) Debug.Log($"Updating Tile {x},{y} ({typeTile}) Visual");
        SetTileVisual(x, y, skinTiles.GetVisual(typeTile), color);
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

            bool anim = visualTile.sprite != sprite;
            visualTile.sprite = sprite;
            visualTile.color = color;

            if (anim)
            {
                visualTile.transform.localScale = Vector3.one * 0.7f;
                LeanTween.scale(visualTile.gameObject, Vector3.one, 0.5f);
            } else
                visualTile.transform.localScale = Vector3.one;
        }
    }
    public void SetTileVisual(int x, int y, Sprite sprite) { SetTileVisual(x, y, sprite, Color.black); }

    private SpriteRenderer SpawnVisuals(int x, int y)
    {
        SpriteRenderer newTile = new GameObject($"{x},{y}").AddComponent<SpriteRenderer>();
        newTile.color = Color.black;
        newTile.transform.parent = transformLevel;
        newTile.transform.localPosition = grid.CellToWorld(x, y);
        return newTile;
    }
}