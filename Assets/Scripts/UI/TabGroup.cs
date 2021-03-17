/* based on tutorial by GameDevGuide
 * source: https://www.youtube.com/watch?v=211t6r12XPQ
 * */

using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> tabButtons;
    private List<TabButton> tabButtonsTemp;
    private TabButton selectedButton;

    public void Subscribe(TabButton button)
    {
        if (button != null)
        {
            if (tabButtons == null) tabButtons = new List<TabButton>();
            if (tabButtonsTemp == null) tabButtonsTemp = new List<TabButton>();

            int buttonIndex = button.Index;
            if (buttonIndex >= 0)
            {
                if (buttonIndex < tabButtons.Count)
                {
                    if (tabButtons[buttonIndex] == null)
                        tabButtons[buttonIndex] = button;
                    else
                        tabButtonsTemp.Add(button);
                } else
                {
                    tabButtons.AddRange(new TabButton[buttonIndex - (tabButtons.Count - 1)]);
                    tabButtons[buttonIndex] = button;
                }
            } else
                tabButtonsTemp.Add(button);
        }
    }

    private void Start()
    {
        tabButtons.AddRange(tabButtonsTemp);
        tabButtons.RemoveAll(item => item == null);
        Debug.Log(tabButtons.Count);

        bool activated = false;
        for (int i = 0; i < tabButtons.Count && !activated; i++)
            if (tabButtons[i] && !tabButtons[i].IsLocked)
            {
                activated = true;
                OnTabSelected(tabButtons[i]);
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