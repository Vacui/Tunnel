using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetValue : MonoBehaviour {

        [SerializeField, Tooltip("Is mandatory to add {0} for the message")] private string stringFormat = "{0}";
        public void GetValueToString(float value) {
            GetComponent<TextMeshProUGUI>().text = string.Format(stringFormat, value);
        }
    }
}