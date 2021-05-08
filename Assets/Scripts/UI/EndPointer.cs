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

        [Header("Text")]
        [SerializeField] private RectTransform textRectTransform;
        [SerializeField] private float textDistance;

        [Header("Icon")]
        [SerializeField] private RectTransform offScreenIconRectTransform;
        [SerializeField] private RectTransform onScreenIconRectTransform;

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
                if (true) {
                    pointerRectTransform.gameObject.SetActive(true);

                    Vector3 fromPosition = playerTransform.position.RemoveZ();
                    Vector3 toPosition = targetPosition.RemoveZ();
                    Debug.DrawLine(fromPosition, toPosition);

                    Vector3 dir = (toPosition - fromPosition).normalized;

                    Vector3 pointerPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);

                    bool isOffScreen =
                        pointerPositionScreenPoint.x <= 0 ||
                        pointerPositionScreenPoint.x >= Screen.width ||
                        pointerPositionScreenPoint.y <= 0 ||
                        pointerPositionScreenPoint.y >= Screen.height - borderTop;

                    if (isOffScreen) {
                        pointerPositionScreenPoint.x = Mathf.Clamp(pointerPositionScreenPoint.x, borderLeft, Screen.width - borderRight);
                        pointerPositionScreenPoint.y = Mathf.Clamp(pointerPositionScreenPoint.y, borderBottom, Screen.height - borderTop);
                    }

                    Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(pointerPositionScreenPoint);
                    pointerRectTransform.position = pointerWorldPosition;
                    pointerRectTransform.localPosition = pointerRectTransform.localPosition.RemoveZ();

                    offScreenIconRectTransform.gameObject.SetActive(isOffScreen);
                    onScreenIconRectTransform.gameObject.SetActive(!isOffScreen);

                    if (isOffScreen) {
                        float angle = MyUtils.GetAngleFromVectorFloat(dir);
                        offScreenIconRectTransform.localEulerAngles = new Vector3(0, 0, angle);
                    }

                    textRectTransform.position = Camera.main.ScreenToWorldPoint(pointerPositionScreenPoint - (dir * textDistance));
                    textRectTransform.localPosition = textRectTransform.localPosition.RemoveZ();
                }
            } else {
                pointerRectTransform.gameObject.SetActive(false);
            }
        }
    }
}