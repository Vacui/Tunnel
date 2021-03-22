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
    private GridXY<GameObject> gridUnknown;
    private Transform transformLevel;

    [SerializeField] private PlayerController prefabPlayer;
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
            Debug.Log("Loading level");

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
        Debug.Log($"Initializing level");
        gridLevel = new GridXY<TileType>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, TileType.NULL);
        gridUnknown = new GridXY<GameObject>(width, height, 1.1f, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * -1.1f, null);
    }

    public void GenerateLevel(List<int> cells)
    {
        Debug.Log("Generating level");
        for (int i = 0; i < cells.Count; i++)
        {
            gridLevel.CellNumToCell(i, out int x, out int y);
            CreateTile(x, y, (TileType)cells[i]);
        }
    }

    private void CreateTile(int x, int y, TileType type)
    {
        if (gridLevel.GetTile(x, y) == TileType.NULL)
        {
            if (type != TileType.Player && type != TileType.Goal)
            {
                if (skinTiles)
                {
                    LevelSkin.TileSkin skinTile = skinTiles.GetUnknownSkin();
                    if (skinTile != null)
                    {
                        gridUnknown.SetTile(x, y, SpawnTile(x, y, skinTile.skin, 1));
                    }
                }
            }
            if (type != TileType.Player)
            {
                gridLevel.SetTile(x, y, type);

                if (skinTiles)
                {
                    LevelSkin.TileSkin skinTile = skinTiles.GetSkin(type);
                    if (skinTile != null)
                    {
                        SpawnTile(x, y, skinTile.skin, 0);
                    }
                }
            } else
            {
                CreateTile(x, y, TileType.NULL);
                if (prefabPlayer != null)
                {
                    PlayerController player;
                    if (!FindObjectOfType<PlayerController>())
                        player = Instantiate(prefabPlayer).GetComponent<PlayerController>();
                    else
                        player = FindObjectOfType<PlayerController>();
                    if (player)
                    {
                        player.MoveToCell(x, y, true);
                    }
                }
            }
        }
    }

    private GameObject SpawnTile(int x, int y, Sprite sprite, int sortingOrder)
    {
        SpriteRenderer newTile = new GameObject($"{x},{y}").AddComponent<SpriteRenderer>();
        newTile.transform.parent = transformLevel;
        newTile.transform.localPosition = gridLevel.CellToWorld(x, y);
        newTile.sprite = sprite;
        newTile.sortingOrder = sortingOrder;
        return newTile.gameObject;
    }

    private void DiscoverTile(int x, int y)
    {
        if (gridUnknown != null)
            if (gridUnknown.CellIsValid(x, y))
            {
                GameObject tileToDestroy = gridUnknown.GetTile(x, y);
                if (tileToDestroy != null)
                    Destroy(tileToDestroy);
            }
    }
}