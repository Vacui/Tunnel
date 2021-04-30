using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [DisallowMultipleComponent]
    public class LevelEditor : MonoBehaviour
    {
        [System.Serializable]
        public class LevelEditorHistoryElement
        {
            public Vector2 cell;
            public Element prevType;
            public Element newType;

            public LevelEditorHistoryElement(Vector2 cell, Element prevType, Element newType)
            {
                this.cell = cell;
                this.prevType = prevType;
                this.newType = newType;
            }
        }

        public static LevelEditor main;

        private Element selectedTileType = Element.NULL;

        public List<LevelEditorHistoryElement> history;

        public void SelectTile(Element type) { selectedTileType = type; Debug.Log($"Selecting Tile {type}"); }

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mouseWorldPosition = MyUtils.GetMouseWorldPosition();
                Debug.Log(mouseWorldPosition);
                LevelManager.main.Grid.WorldToCell(mouseWorldPosition, out int x, out int y);
                Debug.Log($"{x},{y}");

                LevelEditorHistoryElement lastHistory = history.Last();
                if (lastHistory == null || (lastHistory.cell != new Vector2(x, y) || lastHistory.newType != selectedTileType))
                    history.Add(new LevelEditorHistoryElement(new Vector2(x, y), LevelManager.main.Grid.GetTile(x, y), selectedTileType));

                LevelManager.main.Grid.SetTile(x, y, selectedTileType);

            }
        }
    }
}