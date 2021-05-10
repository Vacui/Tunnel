using UnityEngine;

public class ScaleTween : TweenScript {
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 to;

    protected override void ApplyTweenTypeSettings() {
        base.ApplyTweenTypeSettings();

        transform.localScale = from;
        id = LeanTween.scale(objectToAnimate, to, duration).id;
    }
}