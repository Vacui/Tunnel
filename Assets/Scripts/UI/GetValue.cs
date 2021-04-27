using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetValue : MonoBehaviour {
        public void GetValueToString(float value) {
            GetComponent<TextMeshProUGUI>().text = value.ToString();
        }
    }
}