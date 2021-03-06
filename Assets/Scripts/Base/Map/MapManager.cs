using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public class Seed {
        public int width;
        public int height;
        public List<string> cells;

        public bool isValid { get; private set; }

        public Seed(string seed) {

            width = -1;
            height = -1;
            cells = new List<string>();

            seed = seed.Trim();
            List<string> seedParts = seed.Split('/').ToList();

            isValid = false;

            if (seedParts.Count == 3) {
                if (int.TryParse(seedParts[0], out width)) {
                    if (int.TryParse(seedParts[1], out height)) {
                        List<string> tiles = seedParts[2].Split('-').ToList();
                        if (tiles.Count == width * height) {
                            cells = tiles;
                            isValid = true;
                        } else {
                            Debug.LogWarning("Error in the seed cells section length.");
                        }
                    } else {
                        Debug.LogWarning("Error in parsing seed height number.");
                    }
                } else {
                    Debug.LogWarning("Error in parsing seed width number.");
                }
            } else {
                Debug.LogWarning("Error in seed number of parts.");
            }

        }

        public void AddBorder(int depth) {
            if (depth > 0) {
                List<string> newCells = new List<string>();
                newCells.AddBlankRange(width + (2 * depth));
                for (int i = 0; i < cells.Count; i++) {
                    if (i % width == 0) {
                        newCells.AddBlankRange(depth);
                    }
                    newCells.Add(cells[i]);
                    if ((i + 1) % width == 0) {
                        newCells.AddBlankRange(depth);
                    }
                }
                newCells.AddBlankRange(width + (2 * depth));
                cells = newCells;
                width += depth * 2;
                height += depth * 2;
            }
        }

        public string GetSeed() {
            string seed = $"{width}/{height}/";
            foreach (string cell in cells) {
                seed += $"{cell}-";
            }
            seed.Trim('-');
            return seed;
        }
    }

    public event System.EventHandler<GridCoordEventArgs> OnGridReady;

    private const float C_CellSize = 4f;

    [SerializeField] private int borderDepth = 0;
    public GridXZ<GridObject> grid { get; private set; }
    [SerializeField] private GameObject[] tilesArray;
    private Transform mapParent;

    private int mapTiles;
    private int mapTilesExplored;
    public float MapExploringPercentage {
        get {
            return mapTilesExplored / (float)mapTiles;
        }
    }

    private void Awake() {
        PlacedObject.OnExploring += (s, e) => { mapTilesExplored++; /*Debug.Log($"Map discovery percentage {System.Math.Truncate((MapExploringPercentage * 100) * 100) / 100}% ({mapTiles}/{mapTilesExplored})");*/ };
    }

    public void LoadMap(Seed seed, Vector3 mapOriginWorldPosition, bool addBorder = false) {

        if (seed.isValid) {

            if (addBorder) seed.AddBorder(borderDepth);

            if (mapParent != null) {
                Destroy(mapParent.gameObject);
            }

            mapParent = new GameObject().transform;
            mapParent.name = "Map Parent";
            mapParent.parent = transform;
            mapParent.localPosition = Vector3.zero;

            int startX = -1;
            int startZ = -1;
            mapTiles = 0;

            grid = new GridXZ<GridObject>(seed.width, seed.height, C_CellSize, mapOriginWorldPosition, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
            grid.OnGridObjectChanged += OnGridObjectChanged;

            for (int i = 0; i < seed.cells.Count; i++) {
                int value;
                grid.GetXZ(i, out int x, out int z);

                if (seed.cells[i] != "") {
                    if (int.TryParse(seed.cells[i], out value)) {
                        value++;
                        if (value < tilesArray.Length) {
                            if (tilesArray[value] != null) {
                                Transform newCellTransform = Instantiate(tilesArray[value], mapParent).transform;
                                newCellTransform.localPosition = grid.GetWorldPosition(x, z);
                                PlacedObject placedObject = newCellTransform.GetComponent<PlacedObject>();
                                grid.GetGridObject(i).SetPlacedObject(placedObject);
                                mapTiles++;
                                if (value == 1) {
                                    startX = x;
                                    startZ = z;
                                    placedObject.Discover();
                                } else if (value == 2) {
                                    placedObject.Discover();
                                }
                            } else {
                                Debug.LogWarning($"Tile prefab {value} is null.");
                            }
                        } else {
                            Debug.LogWarning($"Tile prefab {value} is not present in the tiles array.");
                        }
                    } else {
                        Debug.LogWarning($"Error in parsing seed cell n.{i} content.");
                    }
                } else {
                    //Debug.Log($"The cell n.{i} is empty.");
                    if (tilesArray[0] != null) {
                        Transform newCellTransform = Instantiate(tilesArray[0], mapParent).transform;
                        newCellTransform.localPosition = grid.GetWorldPosition(x, z);
                    } else {
                        Debug.LogWarning($"Blank tile prefab is null.");
                    }
                }
            }

            OnGridReady?.Invoke(this, new GridCoordEventArgs { x = startX, z = startZ });
        }

    }

    public void LoadMapAround(string seed, Vector3 originPosition) {

        Seed mapSeed = new Seed(seed);
        if (mapSeed.isValid) {
            mapSeed.AddBorder(borderDepth);
            LoadMap(mapSeed, (new Vector3(-mapSeed.width / 2.0f + 0.5f, 0, -mapSeed.height / 2.0f + 0.5f) * C_CellSize));
        }

    }

    private void OnGridObjectChanged(object sender, GridCoordEventArgs e) {

    }

}