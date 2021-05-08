using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Graphic))]
    public class GetCameraBackgroundColor : MonoBehaviour {
        [SerializeField] private Camera targetCamera;

        private void Awake() {
#if (UNITY_EDITOR)
            if (targetCamera == null) {
                targetCamera = Camera.main;
                GetCameraColor();
            }
#endif
            GetCameraColor();
        }

        private void Update() {
#if (UNITY_EDITOR)
            GetCameraColor();
#endif
        }

        private void GetCameraColor() {
            if(targetCamera == null) {
                return;
            }

            Graphic targetGraphic = GetComponent<Graphic>();

            if(targetGraphic == null) {
                return;
            }

            targetGraphic.color = targetCamera.backgroundColor;
        }
    }
}