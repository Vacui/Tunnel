using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public class Seed
    {
        public int width;
        public int height;
        public List<int> cells;

        public bool isValid { get; private set; }

        public Seed(string seed)
        {
            width = -1;
            height = -1;
            cells = new List<int>();

            seed = seed.Trim();
            List<string> seedParts = seed.Split('/').ToList();

            isValid = false;

            if (seedParts.Count == 3)
            {
                if (int.TryParse(seedParts[0], out width))
                {
                    if (width > 0)
                    {
                        if (int.TryParse(seedParts[1], out height))
                        {
                            if (height > 0)
                            {
                                if (seedParts[2].Count(c => (c == '-')) == (width * height) - 1)
                                {
                                    List<string> cellString = seedParts[2].Split('-').ToList();
                                    int cell;
                                    for (int i = 0; i < cellString.Count; i++)
                                    {
                                        if (int.TryParse(cellString[i], out cell))
                                        {
                                            cells.Add(cell);
                                        }
                                    }
                                    if (cells.Count == cellString.Count)
                                    {
                                        isValid = true;
                                    }
                                } else { Debug.LogWarning("Error in the seed cells section length."); }
                            } else { Debug.LogWarning("Seed height is less or equal to 0."); }
                        } else { Debug.LogWarning("Error in parsing seed height number."); }
                    } else { Debug.LogWarning("Seed width is less or equal to 0."); }
                } else { Debug.LogWarning("Error in parsing seed width number."); }
            } else { Debug.LogWarning("Error in seed number of parts."); }
        }

        public override string ToString()
        {
            string seed = $"{width}/{height}/";
            foreach (int cell in cells)
            {
                seed += $"{cell}-";
            }
            seed.Trim('-');
            return seed;
        }
    }

    private const float C_CellSize = 4f;

    public static event Action OnLevelNotReady;
    public static event EventHandler<OnLevelNotPlayableEventArgs> OnLevelNotPlayable;
    public class OnLevelNotPlayableEventArgs : EventArgs
    {
        public int width, height;
    }
    public static event Action OnLevelPlayable;
    public static event Action OnLevelReady;

    public static LevelManager Instance;

    public static GridXY<TileType> gridLevel { get; private set; }
    private GridXY<SpriteRenderer> gridLevelVisuals;
    private Transform transformLevel;

    [SerializeField] private Player prefabPlayer;
    private Player player;
    [SerializeField] private LevelSkin skinTiles;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        PlayerController.OnPlayerStartedMove += ((object sender, PlayerController.OnPlayerStartedMoveEventArgs args) => DiscoverTile(args.x, args.y));

        OnLevelNotReady?.Invoke();
    }

    public void LoadLevel(Seed seedLevel)
    {
        if (seedLevel != null && seedLevel.isValid)
        {
            Debug.Log("0. Loading level");

            if (transformLevel)
                Destroy(transformLevel.gameObject);

            transformLevel = Instantiate(new GameObject("level"), Vector3.zero, Quaternion.identity, transform).transform;

            InitializeLevel(seedLevel.width, seedLevel.height);
            GenerateLevel(seedLevel.cells);

            OnLevelNotPlayable?.Invoke(this, new OnLevelNotPlayableEventArgs { width = seedLevel.width, height = seedLevel.height });
        }
    }

    public void InitializeLevel(int width, int height)
    {
        Debug.Log($"1. Initializing level");
        gridLevel = new GridXY<TileType>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, TileType.NULL);
        gridLevelVisuals = new GridXY<SpriteRenderer>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, null);
    }

    public void GenerateLevel(List<int> cells)
    {
        Debug.Log("2. Generating level");
        for (int i = 0; i < cells.Count; i++)
        {
            gridLevel.CellNumToCell(i, out int x, out int y);
            TileType tile = (TileType)cells[i];
            CreateTile(x, y, tile != TileType.Player ? tile : TileType.Empty);
            if (tile == TileType.Player)
            {
                if (player == null)
                    player = Instantiate(prefabPlayer);
                player.MoveToStartCell(x, y);
            }
        }
    }

    private void CreateTile(int x, int y, TileType type)
    {
        Debug.Log($"Creating Tile {x},{y} - {type}.");
        if (gridLevel.GetTile(x, y) == TileType.NULL)
        {
            gridLevel.SetTile(x, y, type);
            if (!gridLevelVisuals.GetTile(x, y))
                gridLevelVisuals.SetTile(x, y, SpawnTileVisual(x, y));
            HideTile(x, y);

            if (type == TileType.Goal)
                DiscoverTile(x, y);
        }
    }

    private SpriteRenderer SpawnTileVisual(int x, int y)
    {
        SpriteRenderer newTile = new GameObject($"{x},{y}").AddComponent<SpriteRenderer>();
        newTile.transform.parent = transformLevel;
        newTile.transform.localPosition = gridLevel.CellToWorld(x, y);
        return newTile;
    }

    public void EnterTile(int x, int y)
    {
        DiscoverTile(x, y);
        Debug.Log($"Entering Tile {x},{y}.");
        SetTileVisualSprite(x, y, TileVisual.Full);
    }

    public void ExitTile(int x, int y)
    {
        Debug.Log($"Exiting Tile {x},{y}.");
        SetTileVisualSprite(x, y, TileVisual.Empty);
    }

    private void HideTile(int x, int y)
    {
        Debug.Log($"Hiding Tile {x},{y}.");
        SetTileVisualSprite(x, y, TileVisual.Unknown);
    }

    private void DiscoverTile(int x, int y)
    {
        Debug.Log($"Discovering Tile {x},{y}.");
        ExitTile(x, y);
    }

    enum TileVisual
    {
        Empty,
        Full,
        Unknown
    }

    private void SetTileVisualSprite(int x, int y, TileVisual visual)
    {
        Debug.Log($"Setting Tile Visual Sprite {x},{y} - {visual}.");
        if (gridLevelVisuals != null)
            if (gridLevelVisuals.CellIsValid(x, y))
                if (skinTiles)
                {
                    LevelSkin.TileSkin skinTile;
                    if (visual == TileVisual.Unknown)
                        skinTile = skinTiles.GetUnknownSkin();
                    else
                        skinTile = skinTiles.GetSkin(gridLevel.GetTile(x, y));

                    if (skinTile != null)
                        gridLevelVisuals.GetTile(x, y).sprite = visual == TileVisual.Full ? skinTile.full : skinTile.empty;
                }
    }
}