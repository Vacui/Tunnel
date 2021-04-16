using UnityEngine;

namespace Level
{
    [DisallowMultipleComponent]
    public class LevelEditor : MonoBehaviour
    {
        private TileType selectedTileType = TileType.NULL;

        public void SelectTile(TileType type) { selectedTileType = type; Debug.Log($"Selecting Tile {type}"); }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && selectedTileType != TileType.NULL)
            {
                Vector3 mouseWorldPosition = MyUtils.GetMouseWorldPosition();
                Debug.Log(mouseWorldPosition);
                Singletons.main.lvlManager.grid.WorldToCell(mouseWorldPosition, out int x, out int y);
                Debug.Log($"{x},{y}");
                Singletons.main.lvlManager.grid.SetTile(x, y, selectedTileType);
            }
        }
    }
}