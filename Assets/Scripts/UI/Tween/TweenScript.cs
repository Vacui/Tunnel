using UnityEngine;

public class TweenScript : MonoBehaviour {
    [SerializeField] protected float time;
    [SerializeField] private LeanTweenType type;

    [SerializeField, Disable, EditorButton(nameof(Execute), activityType: ButtonActivityType.OnPlayMode)] protected int id = -1;

    public void Execute() {
        StopTween();
        ApplyTweenTypeSettings();
        SetTweenSettings();
    }

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

        descr.setLoopType(type);
    }
}