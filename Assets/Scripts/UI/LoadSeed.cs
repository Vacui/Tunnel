using Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class LoadSeed : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private string seed;

        public void OnPointerClick(PointerEventData eventData) {
            LevelManager.main.LoadLevel(seed);
        }
    }
}