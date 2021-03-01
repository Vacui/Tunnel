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
    Visible
}

[System.Serializable]
public class OpenDirections {
    public bool up;
    public bool right;
    public bool down;
    public bool left;

    public bool IsDirectionOpen(Direction dir) {
        bool result = false;

        switch (dir) {
            case Direction.Up: result = up; break;
            case Direction.Right: result = right; break;
            case Direction.Down: result = down; break;
            case Direction.Left: result = left; break;
        }

        return result;
    }

    public Direction GetOtherDirection(Direction dir) {

        if (up && dir != Direction.Up) return Direction.Up;
        if (right && dir != Direction.Right) return Direction.Right;
        if (down && dir != Direction.Down) return Direction.Down;
        if (left && dir != Direction.Left) return Direction.Left;

        return Direction.NULL;
    }
}

public class PlacedObject : MonoBehaviour {

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
                    case Visibility.Visible: newColor = color; break;
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
    public bool IsSafe;
    [SerializeField] private OpenDirections openDirections;
    public OpenDirections OpenDirections {
        get { return openDirections; }
    }

    private void Awake() {
        Status = Visibility.Hidden;
    }

    public void Discover() {
        if (status != Visibility.Visible) Status = Visibility.Discovered;
    }

    public void Enter() {
        Status = Visibility.Visible;
    }

    private void Reset() {
        if (openDirections == null) openDirections = new OpenDirections();
        Status = Visibility.Hidden;
    }

}