using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
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

    public event EventHandler<OnMapReadyEventArgs> OnMapReady;
    public class OnMapReadyEventArgs : EventArgs
    {
        public Vector3Int startPosition;
    }

    public static GridXZ<TileType> gridGame { get; private set; }
    [SerializeField] private Tilemap tilemapGame;

    [SerializeField] private CustomRuleTile customRuleTileBlank;
    [SerializeField] private CustomRuleTile customRuleTileStart;
    [SerializeField] private CustomRuleTile customRuleTileEnd;
    [SerializeField] private CustomRuleTile customRuleTileNode;
    [SerializeField] private CustomRuleTile customRuleTileFacingUp;
    [SerializeField] private CustomRuleTile customRuleTileFacingRight;
    [SerializeField] private CustomRuleTile customRuleTileFacingDown;
    [SerializeField] private CustomRuleTile customRuleTileFacingLeft;

    private Dictionary<TileType, CustomRuleTile> dictionaryCustomRuleTiles;

    private void Awake()
    {
        dictionaryCustomRuleTiles = new Dictionary<TileType, CustomRuleTile>();
        dictionaryCustomRuleTiles.Add(TileType.NULL, customRuleTileBlank);
        dictionaryCustomRuleTiles.Add(TileType.Start, customRuleTileStart);
        dictionaryCustomRuleTiles.Add(TileType.End, customRuleTileEnd);
        dictionaryCustomRuleTiles.Add(TileType.Node, customRuleTileNode);
        dictionaryCustomRuleTiles.Add(TileType.FacingUp, customRuleTileFacingUp);
        dictionaryCustomRuleTiles.Add(TileType.FacingRight, customRuleTileFacingRight);
        dictionaryCustomRuleTiles.Add(TileType.FacingDown, customRuleTileFacingDown);
        dictionaryCustomRuleTiles.Add(TileType.FacingLeft, customRuleTileFacingLeft);
    }

    public void LoadMap(Seed mapSeed)
    {
        if (mapSeed != null && mapSeed.isValid)
        {
            Debug.Log("Loading map");
            gridGame = new GridXZ<TileType>(mapSeed.width, mapSeed.height, 1, Vector3.zero);
            TileType tileType = TileType.NULL;
            Vector3Int startPosition = Vector3Int.zero;
            for (int i = 0; i < mapSeed.cells.Count; i++)
            {
                tileType = (TileType)mapSeed.cells[i];
                gridGame.SetGridObject(i, tileType);
                if (tileType == TileType.Start) startPosition = gridGame.CellNumToCell(i);
            }

            CustomRuleTile ruleTile;
            Vector3Int tilePosition;
            for (int x = 0; x < gridGame.width; x++)
            {
                for (int z = 0; z < gridGame.height; z++)
                {
                    tilePosition = new Vector3Int(x, 0, z);
                    tileType = gridGame.GetGridObject(tilePosition);
                    if (dictionaryCustomRuleTiles.ContainsKey(tileType))
                    {
                        ruleTile = dictionaryCustomRuleTiles[tileType];
                        tilePosition = new Vector3Int(tilePosition.x, tilePosition.z, 0);
                        if (ruleTile != null)
                            tilemapGame.SetTile(tilePosition, ruleTile);
                    }
                }
            }
            OnMapReady?.Invoke(this, new OnMapReadyEventArgs { startPosition = startPosition });
        }
    }
}