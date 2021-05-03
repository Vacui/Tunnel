using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 goalScale;
    [SerializeField] private float scaleTime;
    [SerializeField] private bool loopPingPong;

    int tween = -1;

    public void Execute() {

        if (tween > -1 && LeanTween.isTweening(tween)) {
            LeanTween.cancel(tween, false);
            tween = -1;
        }

        transform.localScale = startScale;
        tween = LeanTween.scale(gameObject, goalScale, scaleTime).id;

        if (loopPingPong) {
            LeanTween.descr(tween).setLoopPingPong();
        }
    }
}
