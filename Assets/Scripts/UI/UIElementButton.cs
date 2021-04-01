using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string elementToShow;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (elementToShow != "")
            Singletons.main.uiManager.ShowTab(elementToShow);
    }
}
