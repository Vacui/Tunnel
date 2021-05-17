using System.Collections.Generic;
using UnityEngine;

namespace Palette {
    [System.Serializable]
    public class PaletteColor {
        public Color primary;
        public Color secondary;
        public Color text;

        public Color GetColor(PaletteColors color) {
            switch (color) {
                case PaletteColors.Primary: return primary;
                case PaletteColors.Secondary: return secondary;
                case PaletteColors.Text: return text;
            }
            return Color.white;
        }
    }

    [System.Serializable]
    public enum PaletteColors {
        Primary,
        Secondary,
        Text
    }

    public class LevelPalette : MonoBehaviour {
        private static PaletteColor color;
        /// <summary>
        /// Current level palette color.
        /// </summary>
        public static PaletteColor Color {
            get { return color; }
            private set {
                color = value;
                UpdatedColor?.Invoke(value);
            }
        }

        [SerializeField] private PaletteColor defaultColor;
        [SerializeField] private List<PaletteColor> colors;
        /// <summary>
        /// Event called when a new color is setted.
        /// </summary>
        public static System.Action<PaletteColor> UpdatedColor;

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
}