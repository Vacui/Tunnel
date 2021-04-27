using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public class SliderButton : MonoBehaviour {
        [SerializeField] Slider slider;
        [SerializeField] float valueToAdd;

        private void OnEnable() {
            GetComponent<Button>().onClick.AddListener(AddValue);
        }

        private void OnDisable() {
            GetComponent<Button>().onClick.RemoveListener(AddValue);
        }

        private void AddValue() {
            if (slider != null) {
                slider.value += valueToAdd;
            } else Debug.LogWarning("Slider is null", gameObject);
        }
    }
}