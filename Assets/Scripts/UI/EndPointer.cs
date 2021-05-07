using Level;
using UnityEngine;
using PlayerLogic;

namespace UI {
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class EndPointer : MonoBehaviour {

        [SerializeField, Disable] private bool isActive = true;
        [SerializeField, Disable] private Vector3 targetPosition;
        [SerializeField] private RectTransform pointerRectTransform;
        [SerializeField] private Transform playerTransform;

        [Header("Border")]
        [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderTop;
        [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderRight;
        [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderBottom;
        [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderLeft;

        private void Start() {
            LevelManager.OnLevelPlayable += (sender, args) => {
                targetPosition = new Vector3(args.endX, args.endY, 0f);
                isActive = true;
            };
            Player.StartedMoveStatic += (sender, args) => isActive = true;
            Player.StoppedMoveStatic += (sender, args) => isActive = false;
        }

        private void Update() {

            if (LevelManager.Main != null && LevelManager.Main.LvlState == LevelManager.LevelState.Playable) {
                if (isActive) {
                    Vector3 fromPosition = playerTransform.position.RemoveZ();
                    Vector3 toPosition = targetPosition.RemoveZ();
                    Debug.DrawLine(fromPosition, toPosition);
                    Vector3 dir = (toPosition - fromPosition).normalized;
                    float angle = MyUtils.GetAngleFromVectorFloat(dir);
                    pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);

                    Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
                    bool isOffScreen =
                        targetPositionScreenPoint.x <= borderLeft ||
                        targetPositionScreenPoint.x >= Screen.width - borderRight||
                        targetPositionScreenPoint.y <= borderBottom ||
                        targetPositionScreenPoint.y >= Screen.height - borderTop;

                    if (isOffScreen) {
                        Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                        cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderLeft, Screen.width - borderRight);
                        cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderBottom, Screen.height - borderTop);

                        Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(cappedTargetScreenPosition);
                        pointerRectTransform.position = pointerWorldPosition;
                        pointerRectTransform.localPosition = pointerRectTransform.localPosition.RemoveZ();

                        ShowPointer();
                    } else {
                        HidePointer();
                    }
                }
            } else {
                HidePointer();
            }
        }

        private void ShowPointer() {
            pointerRectTransform.gameObject.SetActive(true);
        }

        private void HidePointer() {
            pointerRectTransform.gameObject.SetActive(false);
        }
    }
}