using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetName : MonoBehaviour {
        [SerializeField] private Transform parent;

        private void Awake() {
            if (parent == null) {
                parent = transform.parent;
                GetParentNameToString();
            }
        }

#if (UNITY_EDITOR)
        private void Update() {
            GetParentNameToString();
        }
#endif

        private void GetParentNameToString() {
            if (parent != null) {
                GetComponent<TextMeshProUGUI>().text = parent.name;
            }
        }
    }
}