using System.Collections.Generic;
using UnityEngine;

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

    public bool Contains(Direction dir) {
        bool result = false;

        switch (dir) {
            case Direction.Up: result = up; break;
            case Direction.Right: result = right; break;
            case Direction.Down: result = down; break;
            case Direction.Left: result = left; break;
        }

        return result;
    }

    public Direction OtherDirection(Direction dir) {

        if (up && dir != Direction.Up) return Direction.Up;
        if (right && dir != Direction.Right) return Direction.Right;
        if (down && dir != Direction.Down) return Direction.Down;
        if (left && dir != Direction.Left) return Direction.Left;

        return Direction.NULL;
    }
}

public class PlacedObject : MonoBehaviour {

    public static event System.EventHandler OnExploring;

    private static readonly Color C_HiddenColor = Color.white;

    private Visibility status;
    public Visibility Status {
        get { return status; }
        set {
            status = value;
            if (visuals != null) {
                Color newColor = Color.white;
                switch (status) {
                    case Visibility.Hidden: newColor = C_HiddenColor; break;
                    case Visibility.Discovered: newColor = new Color(color.r, color.g, color.b, 0.5f); break;
                    case Visibility.Explored: newColor = color; break;
                }

                foreach (MeshRenderer visual in visuals) {
                    visual.material.color = newColor;
                }

            }
        }
    }

    [Header("Style")]
    public List<MeshRenderer> visuals;
    public Color color;

    [Header("Navigation")]
    [SerializeField] private bool isSafe;
    [SerializeField] private DirectionsList openDirections;
    public DirectionsList OpenDirections {
        get { return openDirections; }
    }

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

    public bool Enter(Direction enteringDirection, ref bool isSafe) {
        bool result = false;

        if(openDirections.Contains(enteringDirection.Opposite())) {
            if (status != Visibility.Explored) {
                Status = Visibility.Explored;
                OnExploring?.Invoke(this, new System.EventArgs { });
            } else {
                Debug.LogWarning("This tile has already been explored.", gameObject);
            }

            isSafe = this.isSafe;
            result = true;
        } else {
            isSafe = true;
        }

        return result;
    }

    public bool Exit(ref Direction exitingDirection) {
        bool result = false;
        if (openDirections.Contains(exitingDirection)) {
            result = true;
        } else {
            if (!isSafe) {
                exitingDirection = openDirections.OtherDirection(exitingDirection.Opposite());
            } else {
                exitingDirection = Direction.NULL;
            }
        }
        return result;
    }

    private void Reset() {
        if (openDirections == null) openDirections = new DirectionsList();
        Status = Visibility.Hidden;
    }

}