using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    [SerializeField] string originName;
    Dictionary<string, UITab> tabs;
    [SerializeField, Disable] List<UITab> history;
    public List<UITab> History { get { return history; } }

    private void Awake()
    {
        tabs = new Dictionary<string, UITab>();
        history = new List<UITab>();
    }

    public void Subscribe(string name, UITab tab)
    {
        name = name.Trim().ToLower();
        if (!tabs.ContainsKey(name))
        {
            tabs.Add(name, tab);
            if (name == originName)
                ShowTab(name);
            else
                tab.Inactive();
        }
    }

    public void ShowTab(string name)
    {
        name = name.Trim().ToLower();
        if (name != "" && tabs.ContainsKey(name) && tabs[name])
        {
            Debug.Log($"Showing tab {name}");

            if (history.Count > 0)
                history.Last().Inactive();

            if (name == originName)
                history = new List<UITab>();

            history.Add(tabs[name]);
            history.Last().Active();
        }
    }

    public void GoBack()
    {
        if (history.Count > 1)
        {
            history.Last().Inactive();
            history.RemoveLast();
        }
        history.Last().Active();

        Debug.Log("Going a tab back");
    }
}
