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

        /// <summary>
        /// Add value to attached slider.
        /// </summary>
        private void AddValue() {

            if(slider == null) {
                Debug.LogWarning("Slider is null", gameObject);
                return;
            }

            slider.value += valueToAdd;
        }
    }
}