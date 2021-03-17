using UnityEngine;

public class UIElement : MonoBehaviour
{
    [SerializeField, Disable] private bool isVisible;
    public bool IsVisible
    {
        get { return isVisible; }
        set
        {
            isVisible = value;
            if (isVisible) Show();
            else Hide();
            MyUtils.SetObjectsActive(showOnVisible, isVisible);
        }
    }

    [SerializeField, Disable] private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            if (isActive) Active();
            else Inactive();
            MyUtils.SetObjectsActive(showOnActive, isActive);
        }
    }

    [SerializeField, Disable] private bool isLocked;
    public bool IsLocked
    {
        get { return isLocked; }
        set
        {
            isLocked = value;
            if (isLocked) Lock();
            else Unlock();
        }
    }

    [Header("Game Objects")]
    [SerializeField] private GameObject[] showOnActive = new GameObject[0];
    [SerializeField] private GameObject[] showOnVisible = new GameObject[0];

    protected virtual void Awake() { }

    public void ToggleVisible() { IsVisible = !IsVisible; }
    public virtual void Show() { isVisible = true; }
    public virtual void Hide() { isVisible = false; }

    public void ToggleActive() { IsActive = !IsActive; }
    public virtual void Active() { isActive = true; }
    public virtual void Inactive() { isActive = false; }

    public void ToggleLock() { IsLocked = !IsLocked; }
    public virtual void Lock() { isLocked = true; }
    public virtual void Unlock() { isLocked = false; }
}