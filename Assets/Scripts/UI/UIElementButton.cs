using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class UIElementButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string elementToShow;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (elementToShow != "")
                Singletons.main.uiManager.ShowTab(elementToShow);
        }
    }
}