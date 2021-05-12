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

        /// <summary>
        /// Update the attached text to the tab name based on the tab group history.
        /// </summary>
        public void UpdateText() {

            if(text == null) {
                Debug.LogWarning("Text component is null", gameObject);
                return;
            }

            if(group == null) {
                Debug.LogWarning("No TabGroup selected", gameObject);
                return;
            }

            if(group.History == null) {
                Debug.LogWarning("Tab Group History has not been initialized", gameObject);
                return;
            }

            if(group.History.Count <= 0) {
                Debug.LogWarning("Tab Group History is empty", gameObject);
                return;
            }

            Tab lastTab = group.History.Last(offset);

            if (lastTab == null) {
                Debug.LogWarning("Tab to select is null", gameObject);
                return;
            }

            text.text = lastTab.GetName();
        }
    }
}