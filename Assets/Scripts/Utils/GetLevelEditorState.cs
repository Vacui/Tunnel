using UnityEngine;

public interface ITestInterface { }

[RequireComponent(typeof(UIElement))]
public class GetLevelEditorState : MonoBehaviour
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
            LevelEditor.OnLevelNotReady += (() => myUIElement.IsVisible = OnLevelNotReady.IsVisible);
            LevelEditor.OnLevelNotReady += (() => myUIElement.IsLocked = OnLevelNotReady.IsLocked);
            LevelEditor.OnLevelNotReady += (() => myUIElement.IsActive = OnLevelNotReady.IsActive);

            LevelEditor.OnLevelNotPlayable += (() => myUIElement.IsVisible = OnLevelNotPlayable.IsVisible);
            LevelEditor.OnLevelNotPlayable += (() => myUIElement.IsLocked = OnLevelNotPlayable.IsLocked);
            LevelEditor.OnLevelNotPlayable += (() => myUIElement.IsActive = OnLevelNotPlayable.IsActive);

            LevelEditor.OnLevelPlayable += (() => myUIElement.IsVisible = OnLevelPlayable.IsVisible);
            LevelEditor.OnLevelPlayable += (() => myUIElement.IsLocked = OnLevelPlayable.IsLocked);
            LevelEditor.OnLevelPlayable += (() => myUIElement.IsActive = OnLevelPlayable.IsActive);

            LevelEditor.OnLevelReady += (() => myUIElement.IsVisible = OnLevelReady.IsVisible);
            LevelEditor.OnLevelReady += (() => myUIElement.IsLocked = OnLevelReady.IsLocked);
            LevelEditor.OnLevelReady += (() => myUIElement.IsActive = OnLevelReady.IsActive);
        }
    }

}