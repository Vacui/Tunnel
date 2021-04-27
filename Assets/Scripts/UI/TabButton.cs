using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class TabButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TabGroup group;
        [SerializeField] private bool goBackButton = false;
        [SerializeField, ShowIf("goBackButton", false)] private string tabToShow;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (group != null) {
                if (goBackButton) {
                    group.GoBack();
                } else {
                    if (tabToShow != "") {
                        group.ShowTab(tabToShow);
                    }
                }
            }
        }
    }
}