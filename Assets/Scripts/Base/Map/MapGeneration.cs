﻿using UnityEngine;

public class MapGeneration : MonoBehaviour {

    public class Seed {
        public int width;
        public int height;
        public string[] cells;

        public bool isValid { get; private set; }

        public Seed(string seed) {

            width = -1;
            height = -1;
            cells = new string[0];

            seed = seed.Trim();
            string[] seedParts = seed.Split('/');

            isValid = false;

            if (seedParts.Length == 3) {
                if (int.TryParse(seedParts[0], out width)) {
                    if (int.TryParse(seedParts[1], out height)) {
                        string[] tiles = seedParts[2].Split('-');
                        if (tiles.Length == width * height) {
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
    }

    public event System.EventHandler<OnGridReadyEventArgs> OnGridReady;
    public class OnGridReadyEventArgs : System.EventArgs {
        public int x;
        public int z;
    }

    private const int C_CellSize = 1;

    public GridXZ<GridObject> grid { get; private set; }
    [SerializeField] private GameObject[] tilesArray;
    private Transform mapParent;

    public void LoadMap(Seed seed, Vector3 mapOriginWorldPosition) {

        if (seed.isValid) {
            if (mapParent != null) {
                Destroy(mapParent.gameObject);
            }

            mapParent = new GameObject().transform;
            mapParent.name = "Map Parent";
            mapParent.parent = transform;
            mapParent.localPosition = mapOriginWorldPosition;

            int startX = -1;
            int startZ = -1;

            grid = new GridXZ<GridObject>(seed.width, seed.height, C_CellSize, Vector2.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
            grid.OnGridObjectChanged += OnGridObjectChanged;

            for (int i = 0; i < seed.cells.Length; i++) {
                int value;
                grid.GetXZ(i, out int x, out int z);

                if (int.TryParse(seed.cells[i], out value)) {
                    if (value < tilesArray.Length) {
                        if (tilesArray[value] != null) {
                            Transform newCellTransform = Instantiate(tilesArray[value], mapParent).transform;
                            newCellTransform.localPosition = grid.GetWorldPosition(x, z);
                            PlacedObject placedObject = newCellTransform.GetComponent<PlacedObject>();
                            grid.GetGridObject(i).SetPlacedObject(placedObject);
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
            }

            OnGridReady?.Invoke(this, new OnGridReadyEventArgs { x = startX, z = startZ });
        }

    }

    public void LoadMapAround(string seed, Vector3 originWorldPosition) {

        Seed mapSeed = new Seed(seed);
        LoadMap(mapSeed, (new Vector3(-mapSeed.width / 2.0f + 0.5f, 0, -mapSeed.height / 2.0f + 0.5f) * C_CellSize) + originWorldPosition);

    }

    private void OnGridObjectChanged(object sender, GridXZ<GridObject>.OnGridObjectChangedEventArgs e) {

    }

}