using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Tab : UIElement
    {
        [SerializeField] private TabGroup group;
        [SerializeField] private Navbar.NavbarShowSettings navbarSettings;

        [SerializeField] private bool useCustomName = false;
        [SerializeField, EnableIf("useCustomName", true)] private string customName = "";

        [SerializeField, ReorderableList] private GameObject[] objToShowOnActive;
        [SerializeField, ReorderableList] private GameObject[] objToHideOnActive;

        protected override void Start()
        {
            base.Start();
            if (group != null) group.Subscribe(GetName(), this);
        }

        protected override void OnActive()
        {
            base.OnActive();
            MyUtils.SetObjectsActive(objToShowOnActive, IsActive);
            MyUtils.SetObjectsActive(objToHideOnActive, !IsActive);
            Navbar.main.Show(navbarSettings);
        }

        protected override void OnInactive()
        {
            base.OnInactive();
            MyUtils.SetObjectsActive(objToShowOnActive, IsActive);
            MyUtils.SetObjectsActive(objToHideOnActive, !IsActive);
        }

        public string GetName() { return useCustomName ? customName : name; }
    }
}