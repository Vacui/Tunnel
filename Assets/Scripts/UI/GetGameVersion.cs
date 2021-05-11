using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetGameVersion : MonoBehaviour {

        /// <summary>
        /// Update the attached text with the game version.
        /// </summary>
        public void UpdateText() {
            GetComponent<TextMeshProUGUI>().text = Application.version;
        }
    }
}