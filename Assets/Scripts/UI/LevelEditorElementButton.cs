using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class LevelEditorElementButton : UIElementButton, IPointerClickHandler
    {
        [SerializeField] private TileType tileType;
        [SerializeField] private LevelEditorElementButtonGroup group;

        protected override void Awake()
        {
            base.Awake();
            if (group != null) group.Subscribe(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnActive()
        {
            base.OnActive();
            Level.LevelEditor.main.SelectTile(tileType);
        }
    }
}