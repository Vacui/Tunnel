using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

    public event System.EventHandler<OnGridReadyEventArgs> OnGridReady;
    public class OnGridReadyEventArgs : System.EventArgs {
        public int x;
        public int z;
    }

    public GridXZ<GridObject> grid { get; private set; }
    [SerializeField] private GameObject[] tilesArray;
    private Transform mapParent;

    public void LoadMap(string seed) {

        if (IsSeedValid(seed, out int width, out int height, out string[] map)) {
            if (mapParent != null) {
                Destroy(mapParent.gameObject);
            }

            mapParent = new GameObject().transform;
            mapParent.name = "Map Parent";
            mapParent.parent = transform;
            mapParent.localPosition = Vector3.zero;

            int startX = -1;
            int startZ = -1;

            grid = new GridXZ<GridObject>(width, height, 10, Vector2.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
            grid.OnGridObjectChanged += OnGridObjectChanged;

            for (int i = 0; i < map.Length; i++) {
                int value;
                grid.GetXZ(i, out int x, out int z);

                if (int.TryParse(map[i], out value)) {
                    if (tilesArray[value] != null) {
                        Transform newCellTransform = Instantiate(tilesArray[value], mapParent).transform;
                        newCellTransform.localPosition = grid.GetWorldPosition(x, z);
                        newCellTransform.localRotation = Quaternion.identity;
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
                        Debug.LogWarning($"Tile prefab {i} is null.");
                    }
                } else {
                    Debug.LogWarning($"Error in parsing seed cell n.{i} content.");
                    Debug.Log(map[i]);
                }
            }

            OnGridReady?.Invoke(this, new OnGridReadyEventArgs { x = startX, z = startZ });
        }

    }

    public bool IsSeedValid(string seed, out int width, out int height, out string[] map) {

        seed = seed.Trim();
        string[] seedParts = seed.Split('/');

        width = -1;
        height = -1;
        map = new string[0];
        bool result = false;

        if (seedParts.Length == 3) {
            if (int.TryParse(seedParts[0], out width)) {
                if (int.TryParse(seedParts[1], out height)) {
                    string[] tiles = seedParts[2].Split('-');
                    if (tiles.Length == width * height) {
                        map = tiles;
                        result = true;
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

        return result;
    }
    public bool IsSeedValid(string seed) {
        return IsSeedValid(seed, out int width, out int height, out string[] map);
    }

    private void OnGridObjectChanged(object sender, GridXZ<GridObject>.OnGridObjectChangedEventArgs e) {

    }

}