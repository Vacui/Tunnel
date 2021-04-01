using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class UITabButton : UIElement, IPointerClickHandler
{
    [SerializeField] private bool goBackButton = false;
    [SerializeField, EnableIf("goBackButton", false)] private string tabToShow;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (goBackButton || tabToShow != "")
            Active();
    }

    protected override void OnActive()
    {
        base.OnActive();
        if (IsActive)
        {
            if (goBackButton)
                Singletons.main.uiManager.GoBack();
            else
                Singletons.main.uiManager.ShowTab(tabToShow);

            Inactive();
        }
    }
}
