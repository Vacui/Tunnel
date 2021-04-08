using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
    private Camera gameCamera;

    private const float MAX_SIZE = 14f;
    private const float MIN_SIZE = 7.7f;


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
        int size = Mathf.Max(width, height);
        gameCamera.orthographicSize = Mathf.Clamp(size - size * 0.3f, MIN_SIZE, MAX_SIZE);
    }

    /// <summary>
    /// Reset the camera orthographic size to default value. USE WITH CAUTION!
    /// </summary>
    private void ResetSize()
    {
        gameCamera.orthographicSize = 5;
    }
}