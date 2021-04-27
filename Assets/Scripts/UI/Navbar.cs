using UnityEngine;
using UnityEngine.Events;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Navbar : MonoBehaviour {
        public static Navbar main;

        [System.Serializable]
        public class NavbarSettings {
            public bool title;
            public bool menu;
            public bool back;

            public void GetSettings(ref bool title, ref bool menu, ref bool back) {
                title = this.title;
                menu = this.menu;
                back = this.back;
            }
        }

        [System.Serializable]
        public enum NavbarShowSettings {
            None,
            Title,
            MenuButton,
            Title_And_MenuButton
        }

        [Header("Components")]
        [SerializeField] private GameObject titleText;
        [SerializeField] private GameObject menuBtn;
        [SerializeField] private GameObject backBtn;

        public UnityEvent OnShow;

        private void Awake() {
            if (main == null) main = this;
            else Destroy(this);

            Show(null);
        }

        public static void Show(NavbarSettings showSettings) {
            if (main != null) {
                bool title = false;
                bool menu = false;
                bool back = false;

                if (showSettings != null) {
                    showSettings.GetSettings(ref title, ref menu, ref back);
                }

                MyUtils.SetObjectActive(main.titleText, title);
                MyUtils.SetObjectActive(main.menuBtn, menu);
                MyUtils.SetObjectActive(main.backBtn, back);
                main.OnShow?.Invoke();
            }
        }
    }
}