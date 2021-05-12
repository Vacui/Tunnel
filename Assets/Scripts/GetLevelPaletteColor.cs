using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GetLevelPaletteColor : MonoBehaviour {

    [SerializeField] private UltEventColor UpdatedColor;

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

            TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer != null) {
                Gradient trailRendererGradient = new Gradient();
                trailRendererGradient.SetKeys(
                    new GradientColorKey[2] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                    new GradientAlphaKey[2] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
                    );
                trailRenderer.colorGradient = trailRendererGradient;
            }
        }

        UpdatedColor?.Invoke(color);
    }
}