using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorTween : TweenScript {
    [SerializeField] private Color from;
    public Color From { get { return from; } set { from = value; } }
    [SerializeField] private Color to;

    private RectTransform targetRectTransform;
    private Image targetImage;

    private void Awake() {
        targetRectTransform = GetComponent<RectTransform>();
        targetImage = GetComponent<Image>();
    }

    protected override void ApplyTweenTypeSettings() {
        base.ApplyTweenTypeSettings();

        if(targetRectTransform == null || targetImage == null) {
            return;
        }

        targetImage.color = from;
        id = LeanTween.color(targetRectTransform, to, time).id;
    }

    public void SetColors(Color from, Color to) {
        this.from = from;
        this.to = to;
    }
}