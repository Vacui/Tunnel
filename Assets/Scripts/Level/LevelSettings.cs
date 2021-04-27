using UnityEngine;

namespace Level {
    [DisallowMultipleComponent]
    public class LevelSettings : MonoBehaviour {
        [Header("Settings")]
        const int DEFAULT_WIDTH = 5;
        const int DEFAULT_HEIGHT = 5;
        [SerializeField, Disable] private int lvlWidth;
        [SerializeField, Disable] private int lvlHeight;

        private void Awake() {
            ResetValues();
        }

        public void ResetValues() {
            lvlWidth = DEFAULT_WIDTH;
            lvlHeight = DEFAULT_HEIGHT;
        }

        public void SetWidth(float value) { lvlWidth = Mathf.RoundToInt(value); }
        public void SetHeight(float value) { lvlHeight = Mathf.RoundToInt(value); }

        public void ApplySettings() {
            LevelGenerator.main.GenerateLevel(lvlWidth, lvlHeight);
        }
    }
}