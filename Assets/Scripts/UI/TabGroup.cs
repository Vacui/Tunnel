/* based on tutorial by GameDevGuide
 * source: https://www.youtube.com/watch?v=211t6r12XPQ
 * */

using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> tabButtons;
    private TabButton selectedButton;
    [SerializeField] PanelGroup panelGroup;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();

        if (button != null)
            if (!tabButtons.Contains(button))
                tabButtons.Add(button);

        if (button.Interactable)
        {
            if (selectedButton == null)
                OnTabSelected(button);
            else
                button.Deselect();
        } else
            button.Lock();
    }

    public void OnTabSelected(TabButton button)
    {
        if (button != null && button != selectedButton)
        {
            if (selectedButton != null)
            {
                selectedButton.Deselect();
            }

            selectedButton = button;
            selectedButton.Select();

            if (panelGroup != null)
                panelGroup.SetPanelIndex(selectedButton.transform.GetSiblingIndex());
        }
    }
}