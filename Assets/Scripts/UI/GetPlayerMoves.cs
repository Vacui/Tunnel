using PlayerLogic;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(TextMeshProUGUI))]
    public class GetPlayerMoves : MonoBehaviour
    {
        private TextMeshProUGUI text;
        private System.EventHandler<Player.PlayerInputEventArgs> UpdateTextDelegate;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            UpdateTextDelegate = (object sender, Player.PlayerInputEventArgs args) => UpdateText(args.moves);
            UpdateText(Player.main.Moves);
        }

        private void OnEnable() { Player.Input += UpdateTextDelegate; }

        private void OnDisable() { Player.Input -= UpdateTextDelegate; }

        private void UpdateText(int moves)
        {
            if (text != null)
            {
                if (moves < 0) moves = 0;
                text.text = $"{moves}";
            }
        }
    }
}