using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class TabButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool goBackButton = false;

        [SerializeField, ShowIf("goBackButton", false)] private string tabToShow;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (goBackButton)
                Singletons.main.uiManager.GoBack();
            else if (tabToShow != "")
                Singletons.main.uiManager.ShowTab(tabToShow);
        }
    }
}