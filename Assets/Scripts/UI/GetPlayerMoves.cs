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

        int tweenId;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            UpdateTextDelegate = (object sender, Player.PlayerInputEventArgs args) => UpdateText(args.moves, true);
        }

        private void OnEnable() { Player.Input += UpdateTextDelegate; }

        private void OnDisable() { Player.Input -= UpdateTextDelegate; }

        private void Start()
        {
            UpdateText(0, false);
        }

        private void UpdateText(int moves, bool animate)
        {
            if (text != null)
            {
                if (moves < 0) moves = 0;
                text.text = $"{moves}";
                if (LeanTween.isTweening(tweenId))
                    LeanTween.cancel(tweenId, false);
                transform.localScale = Vector3.one;
                if (animate)
                    tweenId = LeanTween.scale(gameObject, Vector3.one * 1.3f, 0.3f).setLoopPingPong(1).id;
            }
        }
    }
}