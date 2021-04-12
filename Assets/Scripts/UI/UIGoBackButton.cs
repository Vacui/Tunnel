using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIGoBackButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool updateText = false;
    [SerializeField] private UITab myTab;
    private TextMeshProUGUI text;
    private bool alreadySubscribed = false;

    private void Awake()
    {
        if (updateText && myTab != null)
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();
            if (!alreadySubscribed)
            {
                myTab.OnActiveEvent.AddListener(UpdateText);
                alreadySubscribed = true;
            }
        }
    }

    private void OnEnable()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (text != null && updateText)
            text.text = Singletons.main.uiManager.History.Last(1).GetName();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Singletons.main.uiManager.GoBack();
    }

    private void OnDestroy()
    {
        myTab.OnActiveEvent.RemoveListener(UpdateText);
    }
}
