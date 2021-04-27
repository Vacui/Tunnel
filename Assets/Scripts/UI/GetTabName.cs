using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), ExecuteInEditMode, DisallowMultipleComponent]
    public class GetTabName : MonoBehaviour {
        [SerializeField] TabGroup group;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int offset = 0;

#if (UNITY_EDITOR)
        private void Awake() {
            if (text == null) {
                text = GetComponent<TextMeshProUGUI>();
            }
        }
#endif

        public void UpdateText() {
            if (text != null) {
                if (group != null) {
                    if (group.History != null) {
                        if (group.History.Count > 0) {
                            Tab lastTab = group.History.Last(offset);
                            if (lastTab != null) {
                                text.text = lastTab.GetName();
                            } else Debug.LogWarning("Tab to select is null", gameObject);
                        } else Debug.LogWarning("Tab Group History is empty", gameObject);
                    } else Debug.LogWarning("Tab Group History has not been initialized", gameObject);
                } else Debug.LogWarning("No TabGroup selected", gameObject);
            } else Debug.LogWarning("Text component is null", gameObject);
        }
    }
}