using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(TextMeshProUGUI))]
    public class GetMathOperation : MonoBehaviour {

        [System.Serializable]
        enum MathOperation {
            Multiply
        }

        [SerializeField] private float num1;
        [SerializeField] private float num2;
        [SerializeField] private MathOperation operation;

        public void SetNum1(float value) {
            num1 = value;
            UpdateString();
        }

        public void SetNum2(float value) {
            num2 = value;
            UpdateString();
        }

        public void UpdateString() {
            float value = 0;
            switch (operation) {
                case MathOperation.Multiply: value = num1 * num2; break;
            }
            GetComponent<TextMeshProUGUI>().text = value.ToString();
        }
    }
}