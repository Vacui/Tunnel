using UnityEngine;

public enum MoveStatus {
    NULL,
    MoveAndExit,
    MoveAndStop,
    Stop
}

public class CharacterControllerXZ : MonoBehaviour {

    [SerializeField] private MapManager map;
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

    private void MoveToStart(object sender, GridCoordEventArgs e) {
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
            // variable necessary for debug info
            Direction exitDirection = moveDirection;
            if (currentXZPlacedObject == null || currentXZPlacedObject.Exit(ref exitDirection)) {
                if (map.grid.XZValid(x, z)) {
                    PlacedObject newXZPlacedObject = map.grid.GetGridObject(x, z).GetPlacedObject();

                    if (newXZPlacedObject.Enter(moveDirection, ref isSafe)) {
                        Debug.Log($"Character moving from {this.x},{this.z} to {x},{z} ({moveDirection}).", newXZPlacedObject.gameObject);
                        transform.position = map.grid.GetWorldPosition(x, z);
                        this.x = x;
                        this.z = z;
                        this.moveDirection = moveDirection;

                        if (isSafe) {
                            PlacedObject placedObject;
                            if (map.grid.XZValid(x + 1, z) && map.grid.GetGridObject(x + 1, z).GetPlacedObject(out placedObject)) placedObject.Discover();
                            if (map.grid.XZValid(x - 1, z) && map.grid.GetGridObject(x - 1, z).GetPlacedObject(out placedObject)) placedObject.Discover();
                            if (map.grid.XZValid(x, z + 1) && map.grid.GetGridObject(x, z + 1).GetPlacedObject(out placedObject)) placedObject.Discover();
                            if (map.grid.XZValid(x, z - 1) && map.grid.GetGridObject(x, z - 1).GetPlacedObject(out placedObject)) placedObject.Discover();
                        }

                    } else {
                        Debug.LogWarning($"Character CAN'T enter {x},{z} from {this.x},{this.z} ({moveDirection}).", gameObject);
                    }
                } else {
                    Debug.LogWarning($"Character moving coordinates {x},{z} are NOT valid.", gameObject);
                }
            } else {
                if (moveDirection != Direction.NULL) {
                    Debug.LogWarning($"Character CAN'T exit {this.x},{this.z} to {x},{z} ({moveDirection}).", gameObject);
                    MoveToCell(exitDirection);
                } else {
                    Debug.LogWarning($"Character CAN'T exit {this.x},{this.z} for {x},{z}.", gameObject);
                }
            }
        } else {
            Debug.LogWarning("Character GridManager has no grid.", gameObject);
        }
    }

    private void MoveToCell(Direction dir) {
        int x = this.x;
        int z = this.z;

        DirectionUtils.DirectionToCoord(dir, ref x, ref z);
        MoveToCell(x, z, dir);
    }

}