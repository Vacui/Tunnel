using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class LevelEditorElementButtonGroup : MonoBehaviour
    {
        LevelEditorElementButton selectedButton = null;

        public void Subscribe(LevelEditorElementButton button)
        {
            button.OnActiveEvent += () => ClickOnElement(button);
        }

        private void ClickOnElement(LevelEditorElementButton button)
        {
            if (button != null && selectedButton != button)
            {
                Debug.Log($"Clicked on button {button.name}");

                if (selectedButton != null) selectedButton.Inactive();
                selectedButton = button;
            }
        }
    }
}