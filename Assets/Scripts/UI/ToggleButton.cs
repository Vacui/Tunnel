using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class ToggleButton : UIElementButton
    {
        public override void OnPointerClick(PointerEventData eventData) { ToggleActive(); }
    }
}