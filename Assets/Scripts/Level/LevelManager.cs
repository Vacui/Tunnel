using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
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
    private GridXY<TileVisibility> gridLevelVisibility;
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

        LeanTween.init(1000);

        Player.OnPlayerStartedMove += (object sender, GridCoordsEventArgs args) => DiscoverTile(args.x, args.y);

        Player.OnPlayerStoppedMove += (object sender, GridCoordsEventArgs args) =>
        {
            DiscoverTile(args.x + 1, args.y);
            DiscoverTile(args.x - 1, args.y);
            DiscoverTile(args.x, args.y + 1);
            DiscoverTile(args.x, args.y - 1);
        };

        OnLevelNotReady?.Invoke();
    }

    public void LoadLevel(Seed seedLevel)
    {
        if (seedLevel != null && seedLevel.isValid)
        {
            Debug.Log("0. Loading level...");

            LeanTween.cancelAll();

            if (transformLevel)
                Destroy(transformLevel.gameObject);

            transformLevel = new GameObject("level").transform;
            transformLevel.parent = transform;
            transformLevel.localPosition = Vector3.zero;
            transformLevel.localRotation = Quaternion.identity;

            InitializeLevel(seedLevel.width, seedLevel.height);
            gridLevelVisibility.OnGridObjectChanged += (object sender, GridCoordsEventArgs args) => UpdateTileVisual(args.x, args.y);
            Vector2Int startPos = GenerateLevel(seedLevel.cells);
            gridLevelVisibility.OnGridObjectChanged += (object sender, GridCoordsEventArgs args) => CheckTilesVisibilityAround(args.x, args.y);
            if (startPos.x >= 0 && startPos.y >= 0)
            {
                Debug.Log("Level is ready!");
                if (player == null)
                    player = Instantiate(prefabPlayer);
                player.MoveToStartCell(startPos.x, startPos.y);
            }

            OnLevelNotPlayable?.Invoke(this, new OnLevelNotPlayableEventArgs { width = seedLevel.width, height = seedLevel.height });
        } else
            Debug.LogWarning($"Can't load level from seed {seedLevel.SeedOriginal}");
    }

    public void InitializeLevel(int width, int height)
    {
        Debug.Log($"1. Initializing level...");

        gridLevel = new GridXY<TileType>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * new Vector2(-1.1f, 1.1f), TileType.NULL);

        gridLevelVisuals = new GridXY<SpriteRenderer>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, null);

        gridLevelVisibility = new GridXY<TileVisibility>(width, height, 1, Vector3.zero, TileVisibility.Invisible);
    }

    public Vector2Int GenerateLevel(List<int> cells)
    {
        Debug.Log("2. Generating level...");
        Vector2Int startPos = Vector2Int.one * -1;

        TileType type;

        Debug.Log("2.1. Setting tile types...");
        for (int i = 0; i < cells.Count; i++)
        {
            gridLevel.CellNumToCell(i, out int x, out int y);
            type = (TileType)cells[i];
            gridLevel.SetTile(x, y, type != TileType.Player ? type : TileType.Node);
            if (showDebugLog) Debug.Log($"Setted Tile n.{i} {gridLevel.GetTileToString(x, y)}");
            if (type == TileType.Player)
            {
                startPos.x = x;
                startPos.y = y;
            }
        }

        Debug.Log($"2.2. Creating tile visuals...");
        for (int x = 0; x < gridLevel.width; x++)
            for (int y = 0; y < gridLevel.height; y++)
            {
                type = gridLevel.GetTile(x, y);

                InstantiateTileVisual(x, y, type);

                if (hideLevel)
                {
                    HideTile(x, y);
                    if (type == TileType.Goal)
                        DiscoverTile(x, y);
                    else if (type == TileType.NULL)
                        IsNullTileReadyToVisible(x, y);
                } else
                    DiscoverTile(x, y);
            }

        return startPos;
    }

    private void InstantiateTileVisual(int x, int y, TileType type)
    {
        if (showDebugLog) Debug.Log($"Instantiating Tile {gridLevel.GetTileToString(x, y)} Visual");

        SpriteRenderer newTileVisual = gridLevelVisuals.GetTile(x, y);

        if (!newTileVisual)
        {
            newTileVisual = SpawnTileVisual(x, y);
            gridLevelVisuals.SetTile(x, y, newTileVisual);
        }

        if (newTileVisual)
        {
            newTileVisual.name = gridLevel.GetTileToString(x, y);
            newTileVisual.transform.localScale = Vector3.one * 0.5f;
            LeanTween.scale(newTileVisual.gameObject, Vector3.one, 0.5f);
        }
    }

    private SpriteRenderer SpawnTileVisual(int x, int y)
    {
        SpriteRenderer newTile = new GameObject($"{x},{y}").AddComponent<SpriteRenderer>();
        newTile.color = Color.black;
        newTile.transform.parent = transformLevel;
        newTile.transform.localPosition = gridLevel.CellToWorld(x, y);
        return newTile;
    }

    private void SetTileVisibility(int x, int y, TileVisibility visibility)
    {
        if (gridLevelVisibility.CellIsValid(x, y) && gridLevelVisibility.GetTile(x, y) != visibility)
        {
            if (showDebugLog) Debug.Log($"Setting Tile {gridLevel.GetTileToString(x, y)} Visibility ({visibility})");
            gridLevelVisibility.SetTile(x, y, visibility);
        }
    }
    private void DiscoverTile(int x, int y) { SetTileVisibility(x, y, TileVisibility.Visible); }
    private void HideTile(int x, int y) { SetTileVisibility(x, y, TileVisibility.Invisible); }

    private void UpdateTileVisual(int x, int y)
    {
        if (gridLevelVisibility.CellIsValid(x, y))
        {
            if (showDebugLog) Debug.Log($"Updating Tile {gridLevel.GetTileToString(x, y)} Visual");
            TileVisibility visibility = gridLevelVisibility.GetTile(x, y);
            if (visibility != TileVisibility.ReadyToVisible)
                SetTileVisualSprite(x, y, gridLevelVisibility.GetTile(x, y) == TileVisibility.Invisible);

            if (gridLevelVisuals.CellIsValid(x, y))
            {
                SpriteRenderer srTileVisual = gridLevelVisuals.GetTile(x, y);
                if (srTileVisual)
                {
                    Color visualColor = new Color(0.7529f, 0.2235f, 0.1686f, 1.0f); // red

                    if(visibility == TileVisibility.ReadyToVisible)
                        visualColor = new Color(0.9019f, 0.4941f, 0.1333f, 1.0f); //orange
                    else if(visibility == TileVisibility.Visible)
                        visualColor = new Color(0.1529f, 0.6823f, 0.3764f, 1.0f); //green

                    srTileVisual.color = visualColor;
                } else Debug.LogWarning($"Can't update null SpriteRenderer for Tile {gridLevel.GetTileToString(x, y)}");
            }
        }
    }

    private void SetTileVisualSprite(int x, int y, bool unknown)
    {
        gridLevelVisuals.GetTile(x, y).sprite = unknown || gridLevel.GetTile(x, y) != TileType.NULL ? skinTiles.GetVisual(unknown ? TileType.NULL : gridLevel.GetTile(x, y)) : null;
    }

    private bool IsNullTileReadyToVisible(int x, int y)
    {
        bool isReadyToVisible = false;

        if (gridLevel.CellIsValid(x, y) && gridLevelVisibility.CellIsValid(x, y))
        {
            if (gridLevel.GetTile(x, y) == TileType.NULL && gridLevelVisibility.GetTile(x, y) == TileVisibility.Invisible)
            {
                if (showDebugLog) Debug.Log($"Is Tile {gridLevel.GetTileToString(x, y)} ReadyToVisible?", gridLevelVisuals.GetTile(x, y)?.gameObject);
                isReadyToVisible = true;

                TileType type;
                TileVisibility visibility;

                MyUtils.LoopNeighbours((tX, tY) =>
                {
                    if (isReadyToVisible)
                        if (gridLevel.CellIsValid(tX, tY) && gridLevelVisibility.CellIsValid(tX, tY))
                        {
                            type = gridLevel.GetTile(tX, tY);
                            visibility = gridLevelVisibility.GetTile(tX, tY);
                            isReadyToVisible = visibility == TileVisibility.Visible || type == TileType.NULL;
                            if (showDebugLog) Debug.Log($"{gridLevel.GetTileToString(tX, tY)} ({visibility}) - {isReadyToVisible}");
                        }
                }, x, y, 1, true, false);
                //int tX2, tY2;
                //for (int xT = -1; xT < 2 && isReadyToVisible; xT++)
                //    for (int yT = -1; yT < 2 && isReadyToVisible; yT++)
                //        if ((xT != 0 || yT != 0))
                //        {
                //            tX2 = x + xT; tY2 = y + yT;
                //            Debug.Log($"{tX2},{tY2}");
                //            if (gridLevel.CellIsValid(tX2, tY2) && gridLevelVisibility.CellIsValid(tX2, tY2))
                //            {
                //                type = gridLevel.GetTile(tX2, tY2);
                //                visibility = gridLevelVisibility.GetTile(tX2, tY2);
                //                isReadyToVisible = visibility == TileVisibility.Visible || type == TileType.NULL;
                //                if (showDebugLog) Debug.Log($"{gridLevel.GetTileToString(tX2, tY2)} ({visibility}) - {isReadyToVisible}");
                //            }
                //        }

                if (isReadyToVisible)
                    gridLevelVisibility.SetTile(x, y, TileVisibility.ReadyToVisible);
            } else
                isReadyToVisible = gridLevelVisibility.GetTile(x, y) != TileVisibility.ReadyToVisible;
        }

        return isReadyToVisible;
    }

    private void CheckTilesVisibilityAround(int x, int y)
    {
        TileType type = gridLevel.GetTile(x, y);
        TileVisibility visibility = gridLevelVisibility.GetTile(x, y);

        if (showDebugLog) Debug.Log($"Checking Tiles Visibility around {gridLevel.GetTileToString(x, y)} ({visibility})");

        if (type != TileType.NULL && visibility == TileVisibility.Visible)
        {
            MyUtils.LoopNeighbours((tX, tY) => IsNullTileReadyToVisible(tX, tY), x, y, 1, true, false);
            //for (int xT = -1; xT < 2; xT++)
            //    for (int yT = -1; yT < 2; yT++)
            //        if (xT != 0 || yT != 0)
            //            IsNullTileReadyToVisible(x + xT, y + yT);
        } else if(type == TileType.NULL && visibility == TileVisibility.ReadyToVisible)
        {
            if(showDebugLog) Debug.Log("Checking for cluster completion");

            List<Vector2Int> cellsChecked = new List<Vector2Int>() { };
            bool clusterIsOk = CheckClusterTileVisibility(x, y, ref cellsChecked);
            if (clusterIsOk)
                foreach (Vector2Int cell in cellsChecked)
                    DiscoverTile(cell.x, cell.y);
        }
    }

    private bool CheckClusterTileVisibility(int x, int y, ref List<Vector2Int> cellsChecked)
    {
        Vector2Int cell = new Vector2Int(x, y);

        if (!cellsChecked.Contains(cell))
            cellsChecked.Add(cell);

        bool result = gridLevelVisibility.CellIsValid(x, y) && gridLevelVisibility.GetTile(x, y) != TileVisibility.Invisible;

        if (showDebugLog) Debug.Log($"Checking cluster Tile {gridLevel.GetTileToString(x, y)} ({gridLevelVisibility.GetTile(x, y)}) = {result}");

        //MyUtils.LoopNeighbours((tX, tY) =>
        //{
        //    if (result)
        //    {
        //        cell = new Vector2Int(tX, tY);
        //        if (gridLevel.CellIsValid(cell.x, cell.y) && gridLevelVisibility.CellIsValid(cell.x, cell.y))
        //            if (!cellsChecked.Contains(cell))
        //                if (gridLevel.GetTile(cell.x, cell.y) == TileType.NULL)
        //                    result = CheckClusterTileVisibility(cell.x, cell.y, ref cellsChecked);
        //    }
        //}, x, y, 1, true, true);
        for (int xT = -1; xT < 2 && result; xT++)
            for (int yT = -1; yT < 2 && result; yT++)
                if ((xT != 0 || yT != 0) && (Mathf.Abs(xT) != Mathf.Abs(yT)))
                {
                    cell = new Vector2Int(x + xT, y + yT);
                    if (gridLevel.CellIsValid(cell.x, cell.y) && gridLevelVisibility.CellIsValid(cell.x, cell.y))
                        if (!cellsChecked.Contains(cell))
                            if (gridLevel.GetTile(cell.x, cell.y) == TileType.NULL && gridLevelVisibility.GetTile(cell.x, cell.y) != TileVisibility.Visible)
                                result = CheckClusterTileVisibility(cell.x, cell.y, ref cellsChecked);
                }

        return result;
    }
}