using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public enum Direction {
    NULL,
    Up,
    Right,
    Down,
    Left
}

public enum Visibility {
    NULL,
    Hidden,
    Discovered,
    Explored
}

[System.Serializable]
public class DirectionsList {
    public bool up;
    public bool right;
    public bool down;
    public bool left;

    public DirectionsList(bool up, bool right, bool down, bool left) {
        this.up = up;
        this.right = right;
        this.down = down;
        this.left = left;
    }
    public DirectionsList() : this(false, false, false, false) { }

    public List<Direction> ToList() {
        List<Direction> result = new List<Direction>();
        if (up) result.Add(Direction.Up);
        if (right) result.Add(Direction.Right);
        if (down) result.Add(Direction.Down);
        if (left) result.Add(Direction.Left);
        return result;
    }

    public bool Contains(Direction dir) {
        return ToList().Contains(dir);
    }

    public Direction OtherDirection(Direction dir) {
        Direction result = Direction.NULL;
        List<Direction> directions = ToList();
        directions.Remove(dir);
        if (directions.Any())
            result = directions[0];

        return result;
    }

    public static DirectionsList operator !(DirectionsList dir) => new DirectionsList(!dir.up, !dir.right, !dir.down, !dir.left);

}

public class PlacedObject : MonoBehaviour {

    public static event System.EventHandler OnExploring;

    private static readonly Color C_HiddenColor = Color.white;

    private Visibility status;
    public Visibility Status {
        get { return status; }
        set {
            status = value;
            if (visual != null) {
                Color newColor = Color.white;
                switch (status) {
                    case Visibility.Hidden: newColor = C_HiddenColor; break;
                    case Visibility.Discovered: newColor = new Color(color.r, color.g, color.b, 0.5f); break;
                    case Visibility.Explored: newColor = color; break;
                }

                visual.material.color = newColor;

            }
        }
    }

    [Header("Style")]
    public MeshRenderer visual;
    public Color color;

    [Header("Navigation")]
    [SerializeField] private bool isSafe;
    [SerializeField] private DirectionsList allDirections;
    [SerializeField] private DirectionsList openDirections;

    private void Awake() {
        Status = Visibility.Hidden;
    }

    public void Discover() {
        if (status == Visibility.Hidden) {
            Status = Visibility.Discovered;
        } else {
            Debug.LogWarning("This tile cannot been discovered because it is not hidden.", gameObject);
        }
    }

    public bool Enter(Direction enteringDirection, ref bool isSafe, ref bool needToWait) {
        bool result = false;
        isSafe = true;

        if(allDirections.Contains(enteringDirection.Opposite())) {
            if (!openDirections.Contains(enteringDirection.Opposite())) {
                visual.transform.rotation = Quaternion.Euler(visual.transform.rotation.eulerAngles + new Vector3(0, 0, 90));
                openDirections = !openDirections;
            }

            if (status != Visibility.Explored) {
                Status = Visibility.Explored;
                OnExploring?.Invoke(this, new System.EventArgs { });
            } else {
                Debug.LogWarning("This tile has already been explored.", gameObject);
            }

            isSafe = this.isSafe;
            result = true;
        }

        return result;
    }

    public bool Exit(ref Direction exitingDirection) {
        bool result = false;
        if (openDirections.Contains(exitingDirection)) {
            result = true;
        } else {
            if (!isSafe) {
                exitingDirection = allDirections.OtherDirection(exitingDirection.Opposite());
            } else {
                exitingDirection = Direction.NULL;
            }
        }
        return result;
    }

    private void Reset() {
        if (allDirections == null) allDirections = new DirectionsList();
        Status = Visibility.Hidden;
    }

}