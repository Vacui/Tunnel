using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public abstract class UIElement : MonoBehaviour {
        [SerializeField, Disable, EditorButton("ToggleActive", "Toggle Active", activityType: ButtonActivityType.Everything)] private bool isActive = false;
        public bool IsActive {
            get { return isActive; }
            private set {
                isActive = IsLocked ? false : value;
                if (isActive) {
                    OnActive();
                    OnActiveEvent?.Invoke();
                } else {
                    OnInactive();
                    OnInactiveEvent?.Invoke();
                }
            }
        }

        private bool isLocked = false;
        public bool IsLocked {
            get { return isLocked; }
            private set {
                isLocked = value;
                if (isLocked) {
                    OnLock();
                    OnLockEvent?.Invoke();
                    IsActive = false;
                } else {
                    OnUnlock();
                    OnUnlockEvent?.Invoke();
                }
            }
        }

        [Header("Events")]
        public UltEvent OnActiveEvent;
        public UltEvent OnInactiveEvent;
        public UltEvent OnLockEvent;
        public UltEvent OnUnlockEvent;

        public void Active() {
            IsActive = IsLocked ? false : true;
        }
        protected virtual void OnActive() { }

        public void Inactive() {
            IsActive = false;
        }
        protected virtual void OnInactive() { }

        public void ToggleActive() {
            IsActive = !IsActive;
        }

        public void Lock() {
            IsLocked = true;
        }
        protected virtual void OnLock() { }

        public void Unlock() {
            IsLocked = false;
        }
        protected virtual void OnUnlock() { }

        public void ToggleLock() {
            IsLocked = !IsLocked;
        }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void Update() { }
    }

    public abstract class UIElementButton : UIElement, IPointerClickHandler {
        public virtual void OnPointerClick(PointerEventData eventData) { Active(); }
    }
}