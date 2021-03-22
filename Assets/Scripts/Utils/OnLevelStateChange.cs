using UnityEngine;

public interface ITestInterface { }

[RequireComponent(typeof(UIElement))]
public class OnLevelStateChange : MonoBehaviour
{
    [System.Serializable]
    public class Rule
    {
        public bool IsVisible;
        public bool IsLocked;
        public bool IsActive;
    }

    [SerializeField] private Rule OnLevelNotReady;
    [SerializeField] private Rule OnLevelNotPlayable;
    [SerializeField] private Rule OnLevelPlayable;
    [SerializeField] private Rule OnLevelReady;

    private UIElement myUIElement;

    private void Awake()
    {
        myUIElement = GetComponent<UIElement>();

        if (myUIElement != null)
        {
            LevelManager.OnLevelNotReady += (() => myUIElement.SetValues(OnLevelNotReady.IsVisible, OnLevelNotReady.IsActive, OnLevelNotReady.IsLocked));
            LevelManager.OnLevelNotPlayable += ((object sender, LevelManager.OnLevelNotPlayableEventArgs args) => myUIElement.SetValues(OnLevelNotPlayable.IsVisible, OnLevelNotPlayable.IsActive, OnLevelNotPlayable.IsLocked));
            LevelManager.OnLevelPlayable += (() => myUIElement.SetValues(OnLevelPlayable.IsVisible, OnLevelPlayable.IsActive, OnLevelPlayable.IsLocked));
            LevelManager.OnLevelReady += (() => myUIElement.SetValues(OnLevelReady.IsVisible, OnLevelReady.IsActive, OnLevelReady.IsLocked));
        }
    }

}