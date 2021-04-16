using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class SelectTileType : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TileType tileType;

        public void OnPointerClick(PointerEventData eventData) { Singletons.main.lvlEditor.SelectTile(tileType); }
    }
}