using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GetLevelPaletteColor : MonoBehaviour {
    private void OnEnable() {
        LevelPalette.UpdatedColor += UpdateColor;
        UpdateColor(LevelPalette.Color);
    }

    private void OnDisable() {
        LevelPalette.UpdatedColor -= UpdateColor;
    }

    private void UpdateColor(Color color) {
        if (GetComponent<RectTransform>()) {
            Graphic graphic = GetComponent<Graphic>();
            if (graphic != null) {
                graphic.color = color;
            }
        } else {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                spriteRenderer.color = color;
            }

            Tilemap tilemap = GetComponent<Tilemap>();
            if (tilemap != null) {
                tilemap.color = color;
            }
        }
    }
}