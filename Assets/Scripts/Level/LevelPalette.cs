using Level;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class LevelPalette : MonoBehaviour {
    [System.Serializable]
    public sealed class LevelPaletteColorChangeEvent : UltEvent<Color> { }
    private static Color color;
    public static Color Color {
        get { return color; }
        private set {
            color = value;
            UpdatedColor?.Invoke(value);
        }
    }

    [SerializeField] private Color defaultColor;
    [SerializeField] private List<Color> colors;
    public static System.Action<Color> UpdatedColor;

    private void Awake() {
        Color = defaultColor;
    }

    public void RandomColor() {
        Color = colors[Random.Range(0, colors.Count - 1)];
    }
}