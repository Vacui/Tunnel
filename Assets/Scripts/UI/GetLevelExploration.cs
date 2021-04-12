using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GetLevelExploration : MonoBehaviour
{
    private TextMeshProUGUI text;
    private System.EventHandler<LevelFog.DiscoveredTileEventArgs> UpdateTextDelegate;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        UpdateTextDelegate = (object sender, LevelFog.DiscoveredTileEventArgs args) => UpdateText(args.levelExplorationPercentage);
        UpdateText(Singletons.main.lvlFog.LevelExplorationPercentage);
    }

    private void OnEnable()
    {
        Singletons.main.lvlFog.DiscoveredTile += UpdateTextDelegate;
    }

    private void OnDisable()
    {
        Singletons.main.lvlFog.DiscoveredTile -= UpdateTextDelegate;
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