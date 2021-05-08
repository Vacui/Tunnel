using Level;
using UnityEngine;
using PlayerLogic;

[DisallowMultipleComponent]
public class EndPointer : MonoBehaviour {

    [SerializeField, Disable, EditorButton(nameof(TogglePointer), "Toggle Pointer", activityType: ButtonActivityType.Everything)] private bool isActive = true;
    [SerializeField, Disable] private Vector3 targetPosition;
    [SerializeField, NotNull] private Transform pointerTransform;
    [SerializeField, NotNull] private Transform playerTransform;

    [SerializeField, Clamp(0f, Mathf.Infinity)] private float originWorldDistance;

    [Header("Text")]
    [SerializeField, NotNull] private Transform textTransform;
    [SerializeField, Clamp(0f, Mathf.Infinity)] private float textWorldDistance;

    [Header("Icon")]
    [SerializeField, NotNull] private Transform offScreenIconTransform;
    [SerializeField, NotNull] private Transform onScreenIconTransform;

    [Header("UI Border")]
    [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderTop;
    [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderRight;
    [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderBottom;
    [SerializeField, Clamp(0f, Mathf.Infinity)] private float borderLeft;

    private void Start() {
        LevelManager.OnLevelPlayable += (sender, args) => {
            targetPosition = new Vector3(args.endX, args.endY, 0f);
            isActive = true;
        };

        Player.StartedMoveStatic += (sender, args) => {
            isActive = true;
        };
        Player.StoppedMoveStatic += (sender, args) => {
            RefreshPointer();
            isActive = false;
        };
    }

    private void Update() {
        RefreshPointer();
    }

    private void RefreshPointer() {

        if (!isActive) {
            return;
        }

        if (LevelManager.Main == null || LevelManager.Main.LvlState != LevelManager.LevelState.Playable) {
            HidePointer();
            return;
        }

        Vector3 fromPosition = playerTransform.position.RemoveZ();
        Vector3 toPosition = targetPosition.RemoveZ();
        Debug.DrawLine(fromPosition, toPosition);

        Vector3 dir = (toPosition - fromPosition).normalized;

        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);

        bool isOffScreen =
            targetPositionScreenPoint.x <= borderLeft ||
            targetPositionScreenPoint.x >= Screen.width - borderRight ||
            targetPositionScreenPoint.y <= borderBottom ||
            targetPositionScreenPoint.y >= Screen.height - borderTop;

        Vector3 pointerPosition = toPosition;

        if (isOffScreen) {
            pointerPosition = fromPosition + (dir * originWorldDistance);
        }

        pointerTransform.position = pointerPosition;
        pointerTransform.localPosition = pointerTransform.localPosition.RemoveZ();

        offScreenIconTransform.gameObject.SetActive(isOffScreen);
        onScreenIconTransform.gameObject.SetActive(!isOffScreen);

        if (isOffScreen) {
            float angle = MyUtils.GetAngleFromVectorFloat(dir);
            offScreenIconTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        textTransform.position = pointerPosition + (dir * textWorldDistance);
        textTransform.localPosition = textTransform.localPosition.RemoveZ();

        ShowPointer();
    }

    private void ShowPointer() {
        pointerTransform.gameObject.SetActive(true);
    }

    private void HidePointer() {
        pointerTransform.gameObject.SetActive(false);
    }

    private void TogglePointer() {
        pointerTransform.gameObject.SetActive(!pointerTransform.gameObject.activeSelf);
    }
}