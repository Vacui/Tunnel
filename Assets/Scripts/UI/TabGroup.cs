using System.Collections.Generic;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform))]
    public class TabGroup : MonoBehaviour {
        [SerializeField] private Tab originTab;
        private string OriginName { get { return originTab.GetName(); } }
        private Dictionary<string, Tab> tabs;
        [SerializeField, Disable] private List<Tab> history;
        public List<Tab> History { get { return history; } }

        private void Awake() {
            tabs = new Dictionary<string, Tab>();
            history = new List<Tab>();
        }

        public void Subscribe(string name, Tab tab) {
            name = name.Trim().ToLower();
            if (!tabs.ContainsKey(name)) {
                tabs.Add(name, tab);
                if (name == OriginName) {
                    ShowTab(name);
                } else {
                    tab.Inactive();
                }
            }
        }

        public void ShowTab(string name) {
            name = name.Trim().ToLower();

            if(name == "" || !tabs.ContainsKey(name) || !tabs[name]) {
                return;
            }

            if (history.Count > 0) {
                Tab lastTab = history.Last();
                if (lastTab.GetName() == name) {
                    return;
                }

                Debug.Log($"Hiding tab { lastTab.GetName()}");
                lastTab.Inactive();
            }

            if (name == OriginName) {
                history = new List<Tab>();
            }

            history.Add(tabs[name]);
            Debug.Log($"Showing tab {history.Last().GetName()}");
            history.Last().Active();
        }

        public void GoBack() {
            if (history.Count > 1) {
                history.Last().Inactive();
                history.RemoveLast();
            }
            history.Last().Active();

            Debug.Log("Going a tab back");
        }
    }
}