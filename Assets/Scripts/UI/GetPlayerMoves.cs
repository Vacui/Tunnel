using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GetPlayerMoves : MonoBehaviour
{
    private TextMeshProUGUI text;
    private System.EventHandler<Player.OnPlayerInputEventArgs> UpdateTextDelegate;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        UpdateTextDelegate = (object sender, Player.OnPlayerInputEventArgs args) => UpdateText(args.moves);
        UpdateText(Singletons.main.player.Moves);
    }

    private void OnEnable() { Player.OnPlayerInput += UpdateTextDelegate; }

    private void OnDisable() { Player.OnPlayerInput -= UpdateTextDelegate; }

    private void UpdateText(int moves)
    {
        if (text != null)
        {
            if (moves < 0) moves = 0;
            text.text = $"{moves}";
        }
    }
}