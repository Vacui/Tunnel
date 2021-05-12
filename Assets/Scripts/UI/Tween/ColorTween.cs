using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorTween : TweenScript {
    [SerializeField] private Color from;
    public Color From { get { return from; } set { from = value; } }
    [SerializeField] private Color to;

    private SpriteRenderer targetSpriteRenderer;

    private void Awake() {
        targetSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ApplyTweenTypeSettings() {
        base.ApplyTweenTypeSettings();

        if (targetSpriteRenderer == null) {
            return;
        }

        targetSpriteRenderer.color = from;
        id = LeanTween.color(objectToAnimate, to, duration).id;
    }

    public void SetColors(Color from, Color to) {
        this.from = from;
        this.to = to;
    }
}