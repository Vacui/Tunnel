using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public abstract class UIElement : MonoBehaviour
    {
        [SerializeField, Disable, EditorButton("ToggleActive", "Toggle Active", activityType: ButtonActivityType.Everything)] private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            private set
            {
                isActive = IsLocked ? false : value;
                if (isActive)
                {
                    OnActive();
                    if (activeEvents.HasFlag(UIElementEvents.Active)) onActiveEvent?.Invoke();
                } else
                {
                    OnInactive();
                    if (activeEvents.HasFlag(UIElementEvents.Inactive)) onInactiveEvent?.Invoke();
                }
            }
        }

        [SerializeField] private bool isLocked = false;
        public bool IsLocked
        {
            get { return isLocked; }
            private set
            {
                isLocked = value;
                if (isLocked)
                {
                    OnLock();
                    if(activeEvents.HasFlag(UIElementEvents.Lock)) onLockEvent?.Invoke();
                    IsActive = false;
                } else
                {
                    OnUnlock();
                    if (activeEvents.HasFlag(UIElementEvents.Unlock)) onUnlockEvent?.Invoke();
                }
            }
        }

        [Header("Events")]
        [SerializeField, ReorderableList, ShowIf(nameof(activeEvents), UIElementEvents.Active)] private UnityEvent onActiveEvent;
        public UnityEvent OnActiveEvent { get { return onActiveEvent; } private set { onActiveEvent = value; } }

        [SerializeField, ReorderableList, ShowIf(nameof(activeEvents), UIElementEvents.Inactive)] private UnityEvent onInactiveEvent;
        public UnityEvent OnInactiveEvent { get { return onInactiveEvent; } private set { onInactiveEvent = value; } }

        [SerializeField, ReorderableList, ShowIf(nameof(activeEvents), UIElementEvents.Lock)] private UnityEvent onLockEvent;
        public UnityEvent OnLockEvent { get { return onLockEvent; } private set { onLockEvent = value; } }

        [SerializeField, ReorderableList, ShowIf(nameof(activeEvents), UIElementEvents.Unlock)] private UnityEvent onUnlockEvent;
        public UnityEvent OnUnlockEvent { get { return onUnlockEvent; } private set { onUnlockEvent = value; } }

        [System.Flags] private enum UIElementEvents { Nothing = 0, Active = 1, Inactive = 2, Lock = 4, Unlock = 8 }
        [SerializeField, EnumFlag] private UIElementEvents activeEvents = UIElementEvents.Active | UIElementEvents.Inactive | UIElementEvents.Lock | UIElementEvents.Unlock;

        public void Active() { IsActive = IsLocked ? false : true; }
        protected virtual void OnActive() { }

        public void Inactive() { IsActive = false; }
        protected virtual void OnInactive() { }

        public void ToggleActive() { IsActive = !IsActive; }

        public void Lock() { IsLocked = true; }
        protected virtual void OnLock() { }

        public void Unlock() { IsLocked = false; }
        protected virtual void OnUnlock() { }

        public void ToggleLock() { IsLocked = !IsLocked; }

        protected virtual void Awake() { IsLocked = isLocked; }

        protected virtual void Start() { }

        protected virtual void Update() { }
    }

    public abstract class UIElementButton : UIElement, IPointerClickHandler
    {
        public virtual void OnPointerClick(PointerEventData eventData) { Active(); }
    }
}