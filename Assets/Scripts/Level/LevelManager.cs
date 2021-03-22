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

    public event EventHandler<OnLevelReadyEventArgs> OnLevelReady;
    public class OnLevelReadyEventArgs : EventArgs
    {
        public Vector3Int startPosition;
    }

    public static LevelManager Instance;

    public static GridXY<TileType> gridGame { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadLevel(Seed seedLevel)
    {
        if (seedLevel != null && seedLevel.isValid)
        {
            Debug.Log("Loading level");

            InitializeLevel(seedLevel.width, seedLevel.height);

            Vector3Int startPosition = GenerateLevel(seedLevel.cells);

            OnLevelReady?.Invoke(this, new OnLevelReadyEventArgs { startPosition = startPosition });
        }
    }

    public void InitializeLevel(int width, int height)
    {
        Debug.Log($"Initializing level");
        gridGame = new GridXY<TileType>(width, height, 1, Vector3.zero, TileType.NULL);
    }

    public Vector3Int GenerateLevel(List<int> cells)
    {
        Debug.Log("Generating level");
        TileType tileType = TileType.NULL;
        Vector3Int startPosition = Vector3Int.one * -1;
        Vector3Int tilePosition;
       
        for (int i = 0; i < cells.Count; i++)
        {
            gridGame.CellNumToCell(i, out int x, out int y);
            tilePosition = new Vector3Int(x, 0, y);
            SetTile(tilePosition, (TileType)cells[i]);
            if (tileType == TileType.Start)
                startPosition = tilePosition;
        }

        return startPosition;
    }

    private bool SetTile(Vector3Int position, TileType value)
    {
        return true;
    }
}