using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tunnel
{
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
                                        for(int i = 0; i < cellString.Count; i++)
                                        {
                                            if (int.TryParse(cellString[i], out cell))
                                            {
                                                cells.Add(cell);
                                            }
                                        }
                                        if(cells.Count == cellString.Count)
                                        {
                                            isValid = true;
                                        }
                                    } else
                                    {
                                        Debug.LogWarning("Error in the seed cells section length.");
                                    }
                                } else
                                {
                                    Debug.LogWarning("Seed height is less or equal to 0.");
                                }
                            } else
                            {
                                Debug.LogWarning("Error in parsing seed height number.");
                            }
                        } else
                        {
                            Debug.LogWarning("Seed width is less or equal to 0.");
                        }
                    } else
                    {
                        Debug.LogWarning("Error in parsing seed width number.");
                    }
                } else
                {
                    Debug.LogWarning("Error in seed number of parts.");
                }

            }

            public void AddBorder(int depth)
            {
                //if (depth > 0)
                //{
                //    List<string> newCells = new List<string>();
                //    newCells.AddBlankRange(width + (2 * depth));
                //    for (int i = 0; i < cells.Count; i++)
                //    {
                //        if (i % width == 0)
                //        {
                //            newCells.AddBlankRange(depth);
                //        }
                //        newCells.Add(cells[i]);
                //        if ((i + 1) % width == 0)
                //        {
                //            newCells.AddBlankRange(depth);
                //        }
                //    }
                //    newCells.AddBlankRange(width + (2 * depth));
                //    cells = newCells;
                //    width += depth * 2;
                //    height += depth * 2;
                //}
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

        private GridXZ gridTiles;
        [SerializeField] RuleTileVisual ruleTileVisual;

        private void Awake()
        {
            Tile.RuleTileVisual = ruleTileVisual;
        }

        public void LoadMap(Seed mapSeed)
        {

            if (mapSeed != null && mapSeed.isValid)
            {
                Debug.Log("Loading map");
                gridTiles = new GridXZ(mapSeed.width, mapSeed.height, 1, Vector3.zero);
                for(int i = 0; i < mapSeed.cells.Count; i++)
                {
                    gridTiles.SetTile(i, new Tile((TileType)mapSeed.cells[i] + 1));
                }
            }
        }
    }
}