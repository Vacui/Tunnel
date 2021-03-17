using UnityEngine;

public class UIElement : MonoBehaviour
{
    private bool needInitialize = true;

    [SerializeField] private bool isVisible = true;
    public bool IsVisible
    {
        get { return isVisible; }
        set
        {
            needInitialize = false;
            isVisible = value;
            if (isVisible) Show();
            else Hide();
            MyUtils.SetObjectsActive(showOnVisible, isVisible);
        }
    }

    [SerializeField] private bool isActive = false;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            if (isActive) Active();
            else Inactive();            
        }
    }

    [SerializeField] private bool isLocked = false;
    public bool IsLocked
    {
        get { return isLocked; }
        set
        {
            needInitialize = false;
            isLocked = value;
            if (isLocked) Lock();
            else Unlock();
        }
    }

    [Header("Game Objects")]
    [SerializeField] private GameObject[] showOnActive = new GameObject[0];
    [SerializeField] private GameObject[] showOnVisible = new GameObject[0];

    protected virtual void Awake()
    {
        if (needInitialize)
            SetValues(isVisible, isActive, isLocked);
    }

    public void SetValues(bool isVisible, bool isActive, bool isLocked)
    {
        needInitialize = false;
        IsVisible = isVisible;
        IsActive = isActive;
        IsLocked = isLocked;
    }

    public void ToggleVisible() { IsVisible = !IsVisible; }
    public virtual void Show()
    {
        isVisible = true;
    }
    public virtual void Hide()
    {
        isVisible = false;
    }

    public void ToggleActive() { IsActive = !IsActive; }
    public virtual void Active()
    {
        isActive = true;
        MyUtils.SetObjectsActive(showOnActive, isActive);
    }
    public virtual void Inactive()
    {
        isActive = false;
        MyUtils.SetObjectsActive(showOnActive, isActive);
    }

    public void ToggleLock() { IsLocked = !IsLocked; }
    public virtual void Lock()
    {
        isLocked = true;
    }
    public virtual void Unlock()
    {
        isLocked = false;
    }
}