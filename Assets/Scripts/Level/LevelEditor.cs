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
            public TileType prevType;
            public TileType newType;

            public LevelEditorHistoryElement(Vector2 cell, TileType prevType, TileType newType)
            {
                this.cell = cell;
                this.prevType = prevType;
                this.newType = newType;
            }
        }

        private TileType selectedTileType = TileType.NULL;

        public List<LevelEditorHistoryElement> history;

        public void SelectTile(TileType type) { selectedTileType = type; Debug.Log($"Selecting Tile {type}"); }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mouseWorldPosition = MyUtils.GetMouseWorldPosition();
                Debug.Log(mouseWorldPosition);
                Singletons.main.lvlManager.grid.WorldToCell(mouseWorldPosition, out int x, out int y);
                Debug.Log($"{x},{y}");

                LevelEditorHistoryElement lastHistory = history.Last();
                if (lastHistory == null || (lastHistory.cell != new Vector2(x, y) || lastHistory.newType != selectedTileType))
                    history.Add(new LevelEditorHistoryElement(new Vector2(x, y), Singletons.main.lvlManager.grid.GetTile(x, y), selectedTileType));

                Singletons.main.lvlManager.grid.SetTile(x, y, selectedTileType);
                
            }
        }
    }
}