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
                    onActiveEvent?.Invoke();
                } else
                {
                    OnInactive();
                    onInactiveEvent?.Invoke();
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
                    onLockEvent?.Invoke();
                    IsActive = false;
                } else
                {
                    OnUnlock();
                    onUnlockEvent?.Invoke();
                }
            }
        }

        [Header("Events")]
        [SerializeField, ReorderableList] private UnityEvent onActiveEvent;
        public UnityEvent OnActiveEvent { get { return onActiveEvent; } private set { onActiveEvent = value; } }

        [SerializeField, ReorderableList] private UnityEvent onInactiveEvent;
        public UnityEvent OnInactiveEvent { get { return onInactiveEvent; } private set { onInactiveEvent = value; } }

        [SerializeField, ReorderableList] private UnityEvent onLockEvent;
        public UnityEvent OnLockEvent { get { return onLockEvent; } private set { onLockEvent = value; } }

        [SerializeField, ReorderableList] private UnityEvent onUnlockEvent;
        public UnityEvent OnUnlockEvent { get { return onUnlockEvent; } private set { onUnlockEvent = value; } }

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