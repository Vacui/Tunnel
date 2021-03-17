/* based on tutorial by GameDevGuide
 * source: https://www.youtube.com/watch?v=211t6r12XPQ
 * */

 using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : UIElement, IPointerClickHandler
{
    [SerializeField] private TabGroup tabGroup;

    [Header("Colors")]
    [SerializeField] private Graphic[] graphics;
    [SerializeField] private Color[] activeColors;
    [SerializeField] private Color[] inactiveColors;
    [SerializeField] private Color[] lockedColors;

    protected override void Awake()
    {
        base.Awake();
        if (tabGroup != null)
            tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tabGroup != null && !IsLocked)
            tabGroup.OnTabSelected(this);
    }

    public override void Active()
    {
        if (!IsLocked)
        {
            base.Active();
            UpdateGraphicColor(activeColors);
        }
    }

    public override void Inactive()
    {
        if (!IsLocked)
        {
            base.Inactive();
            UpdateGraphicColor(inactiveColors);
        }
    }

    public override void Lock()
    {
        base.Lock();
        UpdateGraphicColor(lockedColors);
    }

    public override void Unlock()
    {
        base.Unlock();
        Inactive();
    }

    private void UpdateGraphicColor(Color[] colors)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] != null)
                if (i < colors.Length)
                    graphics[i].color = colors[i];
        }        
    }
}