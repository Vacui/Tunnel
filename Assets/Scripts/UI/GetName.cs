using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetName : MonoBehaviour {
        [SerializeField] private Transform parent;

        private void Awake() {
#if (UNITY_EDITOR)
            if (parent == null) {
                parent = transform.parent;
                GetParentNameToString();
            }
#endif
            GetParentNameToString();
        }

        private void Update() {
#if (UNITY_EDITOR)
            GetParentNameToString();
#endif
        }

        private void GetParentNameToString() {
            if (parent != null) {
                GetComponent<TextMeshProUGUI>().text = parent.name;
            }
        }
    }
}