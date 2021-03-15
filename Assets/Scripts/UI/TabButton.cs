using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TabGroup tabGroup;
    [SerializeField] private Image background;

    private void Awake()
    {
        if (tabGroup != null)
            SubscribeToTabGroup();
    }

    public void SetTabGroup(TabGroup tabGroup)
    {
        if(tabGroup == null)
        {
            this.tabGroup = tabGroup;
            SubscribeToTabGroup();
        }
    }

    private void SubscribeToTabGroup()
    {
        tabGroup.Subscribe(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tabGroup != null)
            tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tabGroup != null)
            tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tabGroup != null)
            tabGroup.OnTabSelected(this);
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (background != null)
            background.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        if (background != null)
            background.color = color;
    }
}
