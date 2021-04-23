using UnityEngine;

namespace PlayerLogic
{
    [DisallowMultipleComponent]
    public class Character : MonoBehaviour
    {
        [SerializeField, NotNull] private SpriteRenderer sr = null;

        [Header("Visual")]
        private const float SCALE_SIZE = 0.5f;
        private const float SCALE_SPEED = 0.1f;
        private const float MOVE_SPEED = 0.3f;

        private bool isMoving = false;

        private void Awake()
        {
            Player.StartedMove += (sender, args) =>
            {
                if (!isMoving)
                {
                    sr.transform.localScale = Vector3.one;
                    LeanTween.scale(sr.gameObject, Vector3.one * SCALE_SIZE, SCALE_SPEED);
                    Move();
                }
            };
        }

        private void Move()
        {
            if (Player.movement.Count > 0)
            {
                isMoving = true;
                Vector3Int cell = Player.movement.Dequeue();
                LeanTween.move(gameObject, Level.LevelVisual.main.Tilemap.CellToWorld(cell), MOVE_SPEED).setOnComplete(() => Move());

            } else
            {
                isMoving = false;
                sr.transform.localScale = Vector3.one * SCALE_SIZE;
                LeanTween.scale(sr.gameObject, Vector3.one, SCALE_SPEED);
            }
        }

        //private void HideVisual() { sr.enabled = false; }
        //private void ShowVisual() { sr.enabled = true; }
    }
}