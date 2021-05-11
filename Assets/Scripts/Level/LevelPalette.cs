using System.Collections.Generic;
using UnityEngine;

public class LevelPalette : MonoBehaviour {
    private static Color color;
    /// <summary>
    /// Current level palette color.
    /// </summary>
    public static Color Color {
        get { return color; }
        private set {
            color = value;
            UpdatedColor?.Invoke(value);
        }
    }

    [SerializeField] private Color defaultColor;
    [SerializeField] private List<Color> colors;
    /// <summary>
    /// Event called when a new color is setted.
    /// </summary>
    public static System.Action<Color> UpdatedColor;

    private void Awake() {
        Color = defaultColor;
    }

    /// <summary>
    /// Choose a random color between the setted possibilities.
    /// </summary>
    public void RandomColor() {
        Color = colors[Random.Range(0, colors.Count - 1)];
    }
}