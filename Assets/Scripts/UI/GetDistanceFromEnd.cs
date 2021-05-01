using Level;
using PlayerLogic;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class GetDistanceFromEnd : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;
        private Pathfinding pathfinding;

        private void OnEnable() {
            LevelManager.main.Grid.OnGridCreated += (sender, args) => {
                pathfinding = new Pathfinding(args.width, args.height, false, true);
            };
            Player.StoppedMove += (sender, args) => UpdateValue(args.x, args.y);
        }

        public void UpdateValue(int startX, int startY) {
            if (text != null) {
                List<Pathfinding.PathNode> path = pathfinding.FindPath(new Vector2Int(startX, startY), LevelManager.main.EndCell, LevelManager.main.Grid);
                if (path != null) {
                    text.text = (path.Where(p => LevelManager.main.Grid.GetTile(p.GetCell()).ToDirection() == Direction.All).ToList().Count - 1).ToString();
                } else {
                    text.text = "N/A";
                }
            }
        }
    }
}