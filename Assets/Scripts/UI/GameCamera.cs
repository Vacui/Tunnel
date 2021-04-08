using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
    private Camera gameCamera;

    private void Awake()
    {
        gameCamera = GetComponent<Camera>();
        ResetSize();
    }

    private void OnEnable()
    {
        LevelManager.OnLevelReady += (object sender, LevelManager.OnLevelReadyEventArgs args) => ResizeCamera(args.width, args.height);
    }

    private void ResizeCamera(int width, int height)
    {
        int size = Mathf.Clamp(Mathf.Max(width, height), 0, 20);
        Debug.Log($"Sizing camera for level {width}x{height}");
        if (size > 10)
            gameCamera.orthographicSize = size - size * 0.3f;
        else
            gameCamera.orthographicSize = 7.7f;
    }

    /// <summary>
    /// Reset the camera orthographic size to default value. USE WITH CAUTION!
    /// </summary>
    private void ResetSize()
    {
        gameCamera.orthographicSize = 5;
    }
}