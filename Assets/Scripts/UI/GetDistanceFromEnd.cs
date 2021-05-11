using Level;
using PlayerLogic;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class GetDistanceFromEnd : MonoBehaviour {
        [SerializeField] private TMP_Text text;

        private void Awake() {
#if (UNITY_EDITOR)
            if (text == null) {
                text = GetComponent<TMP_Text>();
            }
#endif
        }

        private void OnEnable() {
            Player.StoppedMoveStatic += (sender, args) => UpdateValue(args.x, args.y);
        }

        /// <summary>
        /// Update the attached text to the distance between the specified cell and the level end.
        /// </summary>
        /// <param name="startX">Start cell X.</param>
        /// <param name="startY">Start cell Y.</param>
        public void UpdateValue(int startX, int startY) {
            if (text == null) {
                return;
            }

            if (!LevelNavigation.IsReady) {
                return;
            }

            List<LevelNavigation.PathNode> path = LevelNavigation.FindPath(new Vector2Int(startX, startY), LevelManager.Main.EndCell);

            if (path != null) {
                text.text = (path.Where(p => LevelManager.Main.Grid.GetTile(p.GetCell()).ToDirection() == Direction.All).ToList().Count - 1).ToString();
            } else {
                text.text = "N/A";
            }
        }
    }
}