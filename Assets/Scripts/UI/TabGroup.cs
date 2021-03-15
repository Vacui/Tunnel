using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabGroup : MonoBehaviour
{
    public enum VisualState
    {
        Idle,
        Hovered,
        Active
    }

    [System.Serializable]
    public enum Transition
    {
        SpriteSwap,
        ColorTint
    }

    [System.Serializable]
    public class UnityTabButtonEvent : UnityEvent<TabButton> { }

    [SerializeField] private UnityTabButtonEvent OnTabButtonChanged;

    private List<TabButton> tabButtons;
    private TabButton selectedButton;

    [SerializeField] private Transition transition;

    [ShowIf(nameof(transition), Transition.SpriteSwap)]
    [SerializeField] private Sprite tabSpriteIdle;
    [ShowIf(nameof(transition), Transition.SpriteSwap)]
    [SerializeField] private Sprite tabSpriteHover;
    [ShowIf(nameof(transition), Transition.SpriteSwap)]
    [SerializeField] private Sprite tabSpriteActive;

    [ShowIf(nameof(transition), Transition.ColorTint)]
    [SerializeField] private Color tabColorIdle;
    [ShowIf(nameof(transition), Transition.ColorTint)]
    [SerializeField] private Color tabColorHover;
    [ShowIf(nameof(transition), Transition.ColorTint)]
    [SerializeField] private Color tabColorActive;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();

        if(button != null)
        {
            if (!tabButtons.Contains(button))
            {
                tabButtons.Add(button);
                SetTabButtonVisual(button, VisualState.Idle);
            }
        }
    }

    public void OnTabEnter(TabButton button)
    {
        if (button != null && button != selectedButton)
            SetTabButtonVisual(button, VisualState.Hovered);
    }

    public void OnTabExit(TabButton button)
    {
        if(button != null && button != selectedButton)
            SetTabButtonVisual(button, VisualState.Idle);
    }

    public void OnTabSelected(TabButton button)
    {
        if (button != null)
        {
            if(selectedButton != null)
                if(button != selectedButton)
                    SetTabButtonVisual(selectedButton, VisualState.Idle);

            selectedButton = button;
            SetTabButtonVisual(selectedButton, VisualState.Active);

            OnTabButtonChanged?.Invoke(selectedButton);
        }
    }

    private void SetTabButtonVisual(TabButton button, VisualState state)
    {
        switch (transition)
        {
            case Transition.SpriteSwap:
                SetTabButtonSpriteVisual(button, state);
                break;
            case Transition.ColorTint:
                SetTabButtonColorVisual(button, state);
                break;
        }
    }

    public void SetTabButtonSpriteVisual(TabButton button, VisualState state)
    {
        Sprite sprite = tabSpriteIdle;
        switch (state)
        {
            case VisualState.Hovered: sprite = tabSpriteHover; break;
            case VisualState.Active: sprite = tabSpriteActive; break;
        }
        button.SetBackgroundSprite(sprite);
    }

    public void SetTabButtonColorVisual(TabButton button, VisualState state)
    {
        Color color = tabColorIdle;
        switch (state)
        {
            case VisualState.Hovered: color = tabColorHover; break;
            case VisualState.Active: color = tabColorActive; break;
        }
        button.SetBackgroundColor(color); 
    }
}
