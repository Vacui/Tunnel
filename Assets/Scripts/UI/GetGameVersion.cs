using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetGameVersion : MonoBehaviour {
        public void UpdateText() {
            GetComponent<TextMeshProUGUI>().text = Application.version;
        }
    }
}