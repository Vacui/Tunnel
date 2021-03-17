/* based on tutorial by GameDevGuide
 * source: https://www.youtube.com/watch?v=211t6r12XPQ
 * */

using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> tabButtons;
    private TabButton selectedButton;

    public void Subscribe(TabButton button)
    {
        if (button != null)
        {
            if (tabButtons == null)
                tabButtons = new List<TabButton>();

            if (!tabButtons.Contains(button))
                tabButtons.Add(button);

            if (selectedButton == null)
                OnTabSelected(button);
            else
                button.Inactive();
        }
    }

    public void OnTabSelected(TabButton button)
    {
        if (button != null && !button.IsLocked && button != selectedButton)
        {
            selectedButton?.Inactive();

            selectedButton = button;
            selectedButton.Active();
        }
    }
}