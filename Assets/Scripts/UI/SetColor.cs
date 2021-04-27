using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SetColor : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Graphic graphic;
        [SerializeField] private Color color;
        private Color baseColor;

        private void Awake() {
            if (graphic != null) {
                baseColor = graphic.color;
            }
        }

        public void SetColorGraphic() {
            if (graphic != null) {
                graphic.color = color;
            }
        }
        public void ResetColorGraphic() {
            if (graphic != null) {
                graphic.color = baseColor;
            }
        }
    }
}