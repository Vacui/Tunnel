using UnityEngine;

public class CharacterControllerXZ : MonoBehaviour {

    [SerializeField] private MapGeneration map;
    private int x;
    private int z;
    public bool isSafe = true;
    private Direction moveDirection = Direction.NULL;

    private void Awake() {
        if (map != null) {
            map.OnGridReady += MoveToStart;
        }
        x = -1;
        z = -1;
    }

    private void Update() {
        if (isSafe) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) MoveToCell(Direction.Up);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveToCell(Direction.Right);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) MoveToCell(Direction.Down);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveToCell(Direction.Left);
        } else {
            MoveToCell(moveDirection);
        }
    }

    private void MoveToStart(object sender, MapGeneration.OnGridReadyEventArgs e) {
        Debug.Log($"Character moving to start {e.x},{e.z}.");
        x = -1;
        z = -1;
        MoveToCell(e.x, e.z);
    }

    private void MoveToCell(int x, int z, Direction moveDirection = Direction.NULL) {
        if (map.grid != null) {
            PlacedObject currentXZPlacedObject = null;
            if (map.grid.XZValid(this.x, this.z)) {
                currentXZPlacedObject = map.grid.GetGridObject(this.x, this.z).GetPlacedObject();
            }
            if (currentXZPlacedObject == null || currentXZPlacedObject.IsDirectionOpen(moveDirection)) {
                if (map.grid.XZValid(x, z)) {
                    PlacedObject newXZPlacedObject = map.grid.GetGridObject(x, z).GetPlacedObject();

                    if (currentXZPlacedObject == null || newXZPlacedObject.IsDirectionOpen(moveDirection.GetOppositeDirection())) {

                        Debug.Log($"Character moving from {this.x},{this.z} to {x},{z} ({moveDirection}).");
                        this.moveDirection = moveDirection;
                        isSafe = newXZPlacedObject.IsSafe();
                        newXZPlacedObject.Discover();
                        if (isSafe) {
                            if (map.grid.XZValid(x + 1, z)) map.grid.GetGridObject(x + 1, z).GetPlacedObject().Discover();
                            if (map.grid.XZValid(x - 1, z)) map.grid.GetGridObject(x - 1, z).GetPlacedObject().Discover();
                            if (map.grid.XZValid(x, z + 1)) map.grid.GetGridObject(x, z + 1).GetPlacedObject().Discover();
                            if (map.grid.XZValid(x, z - 1)) map.grid.GetGridObject(x, z - 1).GetPlacedObject().Discover();
                        }
                        transform.position = map.grid.GetWorldPosition(x, z);
                        this.x = x;
                        this.z = z;

                    } else {
                        Debug.LogWarning($"Character CAN'T enter {x},{z} from {this.x},{this.z} ({moveDirection}).");
                    }
                } else {
                    Debug.LogWarning($"Character moving coordinates {x},{z} are NOT valid.", gameObject);
                }
            } else {
                Debug.LogWarning($"Character CAN'T exit {this.x},{this.z} for {x},{z} ({moveDirection}).");
                MoveToCell(currentXZPlacedObject.GetOtherDirection(moveDirection.GetOppositeDirection()));
            }
        } else {
            Debug.LogWarning("Character GridManager has no grid.", gameObject);
        }
    }

    private void MoveToCell(Direction dir) {

        if (dir != Direction.NULL) {
            switch (dir) {
                default:
                case Direction.Up: MoveToCell(x, z + 1, dir); break;
                case Direction.Right: MoveToCell(x + 1, z, dir); break;
                case Direction.Down: MoveToCell(x, z - 1, dir); break;
                case Direction.Left: MoveToCell(x - 1, z, dir); break;
            }
        }

    }

}