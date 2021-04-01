using UnityEngine;
using UnityEngine.Events;

public abstract class UIElement : MonoBehaviour
{
    [SerializeField, Disable, EditorButton("ToggleActive", "Toggle Active", activityType: ButtonActivityType.Everything)] private bool isActive = false;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = IsLocked ? false : value;
            if (isActive)
            {
                OnActive();
                OnActiveEvent?.Invoke();
            } else
            {
                OnInactive();
                OnInactiveEvent?.Invoke();
            }
        }
    }

    [SerializeField] private bool isLocked = false;
    public bool IsLocked
    {
        get { return isLocked; }
        set
        {
            isLocked = value;
            if (isLocked)
            {
                Lock();
                OnLockEvent?.Invoke();
                IsActive = false;
            } else
            {
                Unlock();
                OnUnlockEvent?.Invoke();
            }
        }
    }

    [SerializeField] private UnityEvent OnActiveEvent;
    [SerializeField] private UnityEvent OnInactiveEvent;
    [SerializeField] private UnityEvent OnLockEvent;
    [SerializeField] private UnityEvent OnUnlockEvent;


    public void ToggleActive() { IsActive = !IsActive; }
    public virtual void OnActive() { }
    public virtual void OnInactive() { }

    public void ToggleLock() { IsLocked = !IsLocked; }
    public virtual void Lock() { }
    public virtual void Unlock() { }

    protected virtual void Awake()
    {
        IsLocked = isLocked;
    }

    protected virtual void Start() { }
}