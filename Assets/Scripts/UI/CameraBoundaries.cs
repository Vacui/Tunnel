using Cinemachine;
using Level;
using UnityEngine;

namespace UI {
    [DisallowMultipleComponent, RequireComponent(typeof(PolygonCollider2D))]
    public class CameraBoundaries : MonoBehaviour {

        private PolygonCollider2D myPolygonCollider2D;
        [SerializeField] private CinemachineConfiner2D cameraConfiner;

        private void Awake() {
            myPolygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        private void Start() {
            LevelManager.Main.Grid.OnGridCreated += (sender, args) => CreateCameraBoundaries(args.width, args.height, args.cellSize);
        }

        private void CreateCameraBoundaries(int width, int height, int cellSize) {
            if (myPolygonCollider2D == null || cameraConfiner == null) {
                return;
            }

            myPolygonCollider2D.pathCount = 1;
            myPolygonCollider2D.SetPath(0, new Vector2[4] {
                new Vector2(width + 1, height + 1) * cellSize,
                new Vector2(-2, height + 1) * cellSize,
                new Vector2(-2, -2) * cellSize,
                new Vector2(width + 1, -2) * cellSize
            });

            cameraConfiner.InvalidateCache();
        }
    }
}