using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
    private Camera gameCamera;

    private void Awake()
    {
        gameCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        LevelManager.OnLevelNotPlayable += (object sender, LevelManager.OnLevelNotPlayableEventArgs args) => ResizeCamera(args.width, args.height);
    }

    private void ResizeCamera(int width, int height)
    {
        int size = Mathf.Clamp(Mathf.Max(width, height), 0, 20);
        if (size > 10)
            gameCamera.orthographicSize = size - size * 0.3f;
        else
            gameCamera.orthographicSize = 7.7f;
    }
}