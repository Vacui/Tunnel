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
        public string SeedOriginal { get; private set; }

        public bool isValid { get; private set; }

        public Seed(string seed)
        {
            width = -1;
            height = -1;
            cells = new List<int>();

            SeedOriginal = seed;
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
                                        if (cellString[i] != "" && int.TryParse(cellString[i], out cell))
                                            cells.Add(cell);
                                        else
                                            cells.Add(0);
                                    }
                                    isValid = cells.Count == cellString.Count;
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
    [SerializeField] private ElementsVisuals skinTiles;

    [Header("Debug")]
    [SerializeField] private bool hideLevel = true;
    [SerializeField] private bool showDebugLog = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Player.OnPlayerStartedMove += ((object sender, Player.GridCoordsEventArgs args) => DiscoverTile(args.x, args.y));
        Player.OnPlayerStoppedMove += ((object sender, Player.GridCoordsEventArgs args) => DiscoverTile(args.x+1, args.y));
        Player.OnPlayerStoppedMove += ((object sender, Player.GridCoordsEventArgs args) => DiscoverTile(args.x-1, args.y));
        Player.OnPlayerStoppedMove += ((object sender, Player.GridCoordsEventArgs args) => DiscoverTile(args.x, args.y+1));
        Player.OnPlayerStoppedMove += ((object sender, Player.GridCoordsEventArgs args) => DiscoverTile(args.x, args.y-1));

        OnLevelNotReady?.Invoke();
    }

    public void LoadLevel(Seed seedLevel)
    {
        if (seedLevel != null && seedLevel.isValid)
        {
            if (showDebugLog) Debug.Log("0. Loading level");

            if (transformLevel)
                Destroy(transformLevel.gameObject);

            transformLevel = new GameObject("level").transform;
            transformLevel.parent = transform;
            transformLevel.localPosition = Vector3.zero;
            transformLevel.localRotation = Quaternion.identity;

            InitializeLevel(seedLevel.width, seedLevel.height);
            GenerateLevel(seedLevel.cells);

            OnLevelNotPlayable?.Invoke(this, new OnLevelNotPlayableEventArgs { width = seedLevel.width, height = seedLevel.height });
        } else
        {
            Debug.LogWarning($"Can't load level from seed {seedLevel.SeedOriginal}.");
        }
    }

    public void InitializeLevel(int width, int height)
    {
        if (showDebugLog) Debug.Log($"1. Initializing level");
        gridLevel = new GridXY<TileType>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, TileType.NULL);
        gridLevelVisuals = new GridXY<SpriteRenderer>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, null);
    }

    public void GenerateLevel(List<int> cells)
    {
        if (showDebugLog) Debug.Log("2. Generating level");
        int startCellX = -1;
        int startCellY = -1;
        for (int i = 0; i < cells.Count; i++)
        {
            gridLevel.CellNumToCell(i, out int x, out int y);
            TileType tile = (TileType)cells[i];
            if (showDebugLog) Debug.Log($"Tile {i} {x},{y} value {cells[i]} = {tile}.");
            CreateTile(x, y, tile != TileType.Player ? tile : TileType.Node);
            if (tile == TileType.Player)
            {
                startCellX = x;
                startCellY = y;
            }
        }

        if(startCellX >= 0 && startCellY >= 0)
        {
            if (player == null)
                player = Instantiate(prefabPlayer);
            player.MoveToStartCell(startCellX, startCellY);
        }
    }

    private void CreateTile(int x, int y, TileType type)
    {
        if (showDebugLog) Debug.Log($"Creating Tile {x},{y} - {type}.");
        if (gridLevel.GetTile(x, y) == TileType.NULL)
        {
            gridLevel.SetTile(x, y, type);

            SpriteRenderer newTileVisual = gridLevelVisuals.GetTile(x, y);

            if (!newTileVisual)
            {
                newTileVisual = SpawnTileVisual(x, y);
                gridLevelVisuals.SetTile(x, y, newTileVisual);
            }

            if (hideLevel)
                HideTile(x, y);
            else
                DiscoverTile(x, y);

            if (type == TileType.Goal)
                DiscoverTile(x, y);

            if (newTileVisual)
            {
                newTileVisual.transform.localScale = Vector3.one * 0.5f;
                LeanTween.scale(newTileVisual.gameObject, Vector3.one, 0.5f);
            }
        }
    }

    private SpriteRenderer SpawnTileVisual(int x, int y)
    {
        SpriteRenderer newTile = new GameObject($"{x},{y}").AddComponent<SpriteRenderer>();
        newTile.transform.parent = transformLevel;
        newTile.transform.localPosition = gridLevel.CellToWorld(x, y);
        return newTile;
    }

    private void HideTile(int x, int y)
    {
        if (showDebugLog) Debug.Log($"Hiding Tile {x},{y}.");
        SetTileVisualSprite(x, y, true);
    }

    private void DiscoverTile(int x, int y)
    {
        if (showDebugLog) Debug.Log($"Discovering Tile {x},{y}.");
        SetTileVisualSprite(x, y, false);
    }

    private void SetTileVisualSprite(int x, int y, bool unknown)
    {
        if (showDebugLog) Debug.Log($"Setting Tile Visual Sprite {x},{y} - Unknown: {unknown}.");
        if (gridLevel != null && gridLevel.CellIsValid(x, y))
            if (gridLevelVisuals != null && gridLevelVisuals.CellIsValid(x, y))
                gridLevelVisuals.GetTile(x, y).sprite = skinTiles.GetVisual(unknown ? TileType.NULL : gridLevel.GetTile(x, y));
    }
}