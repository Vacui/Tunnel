using Level;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(TextMeshProUGUI))]
    public class GetLevelExploration : MonoBehaviour
    {
        private TextMeshProUGUI text;
        private System.EventHandler<LevelFog.ChangedTileVisibilityEventArgs> UpdateTextDelegate;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            UpdateTextDelegate = (object sender, LevelFog.ChangedTileVisibilityEventArgs args) => UpdateText(args.levelExplorationPercentage);
            UpdateText(LevelFog.main.LevelExplorationPercentage);
        }

        private void OnEnable() {
            LevelFog.HiddenTile += UpdateTextDelegate;
            LevelFog.DiscoveredTile += UpdateTextDelegate;
        }

        private void OnDisable() {
            LevelFog.HiddenTile -= UpdateTextDelegate;
            LevelFog.DiscoveredTile += UpdateTextDelegate;
        }

        private void UpdateText(float percentage)
        {
            if (text != null)
            {
                percentage = Mathf.RoundToInt(Mathf.Clamp(percentage, 0, 1) * 100);
                text.text = $"{percentage}%";
            }
        }
    }
}