using UltEvents;
using UnityEngine;

public class TweenScript : MonoBehaviour {
    [SerializeField, NotNull] protected GameObject objectToAnimate;
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("type")] private LeanTweenType easeType;
    [SerializeField, ShowIf(nameof(easeType), LeanTweenType.animationCurve)] private AnimationCurve animationCurve;
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("time")] protected float duration;
    [SerializeField, Clamp(0, Mathf.Infinity)] private float delay;
    [SerializeField] private bool loop;
    [SerializeField, ShowIf(nameof(loop), true)] private bool pingPong;
    [SerializeField, ShowIf(nameof(loop), true), Clamp(-1, 99)] private int loopTimes;
    [SerializeField] private UltEvent onCompleteCallback;
    [SerializeField] private bool destroyOnComplete;

    [SerializeField, Disable, EditorButton(nameof(Execute), activityType: ButtonActivityType.OnPlayMode)] protected int id = -1;

    private void Awake() {
#if (UNITY_EDITOR)
        if (objectToAnimate == null) {
            objectToAnimate = gameObject;
        }
#endif
    }

    /// <summary>
    /// Start the tween.
    /// </summary>
    public void Execute() {
        StopTween();
        ApplyTweenTypeSettings();
        SetTweenSettings();
    }

    /// <summary>
    /// Stop the tween.
    /// </summary>
    public void StopTween() {
        if (id > -1 && LeanTween.isTweening(id)) {
            LeanTween.cancel(id, false);
            id = -1;
        }
    }

    protected virtual void ApplyTweenTypeSettings() { }

    private void SetTweenSettings() {
        if (!LeanTween.isTweening(id)) {
            return;
        }

        LTDescr descr = LeanTween.descr(id);

        if (descr == null) {
            return;
        }

        if (easeType == LeanTweenType.animationCurve) {
            descr.setEase(animationCurve);

        } else {
            descr.setEase(easeType);
        }

        descr.setDelay(delay);

        if (loop) {
            if (pingPong) {
                descr.setLoopPingPong(loopTimes);
            } else {
                descr.setLoopClamp(loopTimes);
            }
        }

        descr.setOnComplete(() => onCompleteCallback?.Invoke());
        descr.setDestroyOnComplete(destroyOnComplete);
    }
}