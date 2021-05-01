using UnityEngine;

namespace Level {
    [DisallowMultipleComponent]
    public class LevelSettings : MonoBehaviour {
        [Header("Settings")]
        private const int DEFAULT_WIDTH = 5;
        private const int DEFAULT_HEIGHT = 5;
        private const int DEFAULT_NODES_PERCENTAGE = 1;
        [SerializeField, Disable] private int lvlWidth;
        [SerializeField, Disable] private int lvlHeight;
        [SerializeField, Disable] private int nodesPercentage;

        private void Awake() {
            lvlWidth = DEFAULT_WIDTH;
            lvlHeight = DEFAULT_HEIGHT;
            nodesPercentage = DEFAULT_NODES_PERCENTAGE;
        }

        public void SetWidth(float value) { lvlWidth = Mathf.RoundToInt(value); }
        public void SetHeight(float value) { lvlHeight = Mathf.RoundToInt(value); }
        public void SetNodesPercentage(float value) { nodesPercentage = Mathf.RoundToInt(value); }

        public void ApplySettings() {
            GridXY<Element> newLevel = LevelGenerator.GenerateLevel(lvlWidth, lvlHeight, nodesPercentage);
            if(newLevel != null) {
                LevelManager.main.LoadLevel(newLevel.ToSeedString());
            } else {
                GameDebug.LogWarning("Level generated is not valid, abort");
            }
        }
    }
}