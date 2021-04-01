using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    [SerializeField] string originName;
    Dictionary<string, UITab> tabs;
    [SerializeField, Disable] List<UITab> history;

    private void Awake()
    {
        tabs = new Dictionary<string, UITab>();
        history = new List<UITab>();
    }

    public void Subscribe(string name, UITab tab)
    {
        if (!tabs.ContainsKey(name))
        {
            tabs.Add(name, tab);
            if (name == originName)
                ShowTab(name);
            else
                tab.IsActive = false;
        }
    }

    public void ShowTab(string name)
    {
        if (name != "" && tabs.ContainsKey(name) && tabs[name])
        {
            Debug.Log($"Showing tab {name}");

            if (history.Count > 0)
                history.Last().IsActive = false;

            if (name == originName)
                history = new List<UITab>();

            history.Add(tabs[name]);
            history.Last().IsActive = true;
        }
    }

    public void GoBack()
    {
        if (history.Count > 1)
        {
            history.Last().IsActive = false;
            history.RemoveLast();
        }
        history.Last().IsActive = true;

        Debug.Log("Going a tab back");
    }
}
