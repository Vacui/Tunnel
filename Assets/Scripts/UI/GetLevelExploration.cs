using Level;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(TextMeshProUGUI))]
    public class GetLevelExploration : MonoBehaviour
    {
        private TextMeshProUGUI text;
        private System.EventHandler<LevelFog.DiscoveredTileEventArgs> UpdateTextDelegate;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            UpdateTextDelegate = (object sender, LevelFog.DiscoveredTileEventArgs args) => UpdateText(args.levelExplorationPercentage);
            UpdateText(LevelFog.main.LevelExplorationPercentage);
        }

        private void OnEnable() { LevelFog.main.DiscoveredTile += UpdateTextDelegate; }

        private void OnDisable() { LevelFog.main.DiscoveredTile -= UpdateTextDelegate; }

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