using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Tab : UIElement {
        [Header("Tab Settings")]
        [SerializeField] private TabGroup group;
        [SerializeField] private bool useCustomName = false;
        [SerializeField, EnableIf("useCustomName", true)] private string customName = "";

        [Header("On Active")]
        [SerializeField] private bool showChildrens;
        [SerializeField, ReorderableList] private GameObject[] objToShowOnActive;
        [SerializeField, ReorderableList] private GameObject[] objToHideOnActive;

        protected override void Start() {
            base.Start();
            if (group != null) {
                group.Subscribe(GetName(), this);
            }
        }

        protected override void OnActive() {
            base.OnActive();
            UpdateChildrens();
            MyUtils.SetObjectsActive(objToShowOnActive, IsActive);
            MyUtils.SetObjectsActive(objToHideOnActive, !IsActive);
        }

        protected override void OnInactive() {
            base.OnInactive();
            UpdateChildrens();
            MyUtils.SetObjectsActive(objToShowOnActive, IsActive);
            MyUtils.SetObjectsActive(objToHideOnActive, !IsActive);
        }

        private void UpdateChildrens() {
            if (showChildrens) {
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(IsActive);
                }
            }
        }

        public string GetName() {
            return (useCustomName ? customName : name).ToLower();
        }
    }
}