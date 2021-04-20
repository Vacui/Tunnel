using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] string originName;
        Dictionary<string, Tab> tabs;
        [SerializeField, Disable] List<Tab> history;
        public List<Tab> History { get { return history; } }

        private void Awake()
        {
            tabs = new Dictionary<string, Tab>();
            history = new List<Tab>();
        }

        public void Subscribe(string name, Tab tab)
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
                    history = new List<Tab>();

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
}