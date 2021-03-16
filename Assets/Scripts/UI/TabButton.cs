/* based on tutorial by GameDevGuide
 * source: https://www.youtube.com/watch?v=211t6r12XPQ
 * */

 using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool interactable = false;
    public bool Interactable { get { return interactable; } }

    [SerializeField] private TabGroup tabGroup;

    [Header("Colors")]
    [SerializeField] private Graphic[] graphics;
    [SerializeField] private Color[] activeColors;
    [SerializeField] private Color[] inactiveColors;
    [SerializeField] private Color[] lockedColors;

    [Header("Game Objects")]
    [SerializeField] private GameObject[] showOnSelected;
    [SerializeField] private GameObject[] hideOnSelected;

    private void Awake()
    {
        if (tabGroup != null)
            tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tabGroup != null && interactable)
            tabGroup.OnTabSelected(this);
    }

    private void UpdateGraphic(Color[] colors, bool show)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] != null)
                if (i < colors.Length)
                    graphics[i].color = colors[i];
        }

        for (int i = 0; i < showOnSelected.Length; i++)
        {
            if (showOnSelected[i] != null)
                showOnSelected[i].SetActive(show);
        }

        for (int i = 0; i < hideOnSelected.Length; i++)
        {
            if (hideOnSelected[i] != null)
                hideOnSelected[i].SetActive(!show);
        }
    }

    public void Select()
    {
        if (interactable)
            UpdateGraphic(activeColors, true);
    }

    public void Deselect()
    {
        if (interactable)
            UpdateGraphic(inactiveColors, false);
    }

    public void Lock()
    {
        interactable = false;
        UpdateGraphic(lockedColors, false);
    }

    public void Unlock()
    {
        interactable = true;
        Deselect();
    }
}