using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Navbar : MonoBehaviour
    {
        private static Navbar instance = null;

        [System.Serializable]
        public enum NavbarShowSettings
        {
            Hide,
            Show,
            OnlyBackBtn,
            OnlyTitle,
            OnlyMenuBtn,
            Back_And_Menu_Btns
        }

        [Header("Components")]
        [SerializeField] private GameObject backBtn;
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject menuBtn;

        public UnityEvent OnShow;

        private void Awake()
        {
            if (instance) Destroy(this);
            else instance = this;
        }

        public static void Show(NavbarShowSettings showSettings)
        {
            switch (showSettings)
            {
                case NavbarShowSettings.Hide: Hide(); break;
                case NavbarShowSettings.Show: Show(true, true, true); break;
                case NavbarShowSettings.OnlyBackBtn: Show(true, false, false); break;
                case NavbarShowSettings.OnlyTitle: Show(false, true, false); break;
                case NavbarShowSettings.OnlyMenuBtn: Show(false, false, true); break;
                case NavbarShowSettings.Back_And_Menu_Btns: Show(true, false, true); break;
            }
        }

        private static void Show(bool backBtn, bool title, bool menuBtn)
        {
            if (instance)
            {
                MyUtils.SetObjectActive(instance.backBtn, backBtn);
                MyUtils.SetObjectActive(instance.title, title);
                MyUtils.SetObjectActive(instance.menuBtn, menuBtn);
                instance.OnShow?.Invoke();
            }
        }

        public static void Hide()
        {
            if (instance)
            {
                MyUtils.SetObjectActive(instance.backBtn, false);
                MyUtils.SetObjectActive(instance.title, false);
                MyUtils.SetObjectActive(instance.menuBtn, false);
            }
        }
    }
}