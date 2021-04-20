using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Navbar : MonoBehaviour
    {
        public static Navbar main;

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
            if (main == null) main = this;
            else Destroy(this);

            Show(false, false, false);
        }

        public void Show(NavbarShowSettings showSettings)
        {
            switch (showSettings)
            {
                case NavbarShowSettings.Hide: Show(false, false, false); break;
                case NavbarShowSettings.Show: Show(true, true, true); break;
                case NavbarShowSettings.OnlyBackBtn: Show(true, false, false); break;
                case NavbarShowSettings.OnlyTitle: Show(false, true, false); break;
                case NavbarShowSettings.OnlyMenuBtn: Show(false, false, true); break;
                case NavbarShowSettings.Back_And_Menu_Btns: Show(true, false, true); break;
            }
        }

        private void Show(bool backBtn, bool title, bool menuBtn)
        {
            MyUtils.SetObjectActive(this.backBtn, backBtn);
            MyUtils.SetObjectActive(this.title, title);
            MyUtils.SetObjectActive(this.menuBtn, menuBtn);
            OnShow?.Invoke();
        }
    }
}