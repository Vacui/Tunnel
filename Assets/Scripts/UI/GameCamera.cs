using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class GameCamera : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;

    private const float MAX_SIZE = 14f;
    private const float MIN_SIZE = 7.7f;

    private const int MAX_WIDTH = 30;
    private const int MAX_HEIGHT = 20;

    [SerializeField, Disable] private bool followPlayer = false;
    [SerializeField] private PolygonCollider2D camConfiner = null;

    private readonly Vector2 offset = new Vector2(1.5f, 3f);

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        ResetSize();
    }

    private void OnEnable()
    {
        LevelManager.OnLevelReady += (object sender, LevelManager.OnLevelReadyEventArgs args) => ResizeCamera(args.width, args.height);
    }

    private void ResizeCamera(int width, int height)
    {
        int size = Mathf.Max(width, height);
        vCam.m_Lens.OrthographicSize = Mathf.Clamp(size - size * 0.3f, MIN_SIZE, MAX_SIZE);
        followPlayer = width > MAX_WIDTH || height > MAX_HEIGHT;
        camConfiner.points = new Vector2[4]
        {
            Singletons.main.lvlManager.grid.CellToWorld(-1, -1) + new Vector2(-offset.x, offset.y),
            Singletons.main.lvlManager.grid.CellToWorld(width, -1) + new Vector2(offset.x, offset.y),
            Singletons.main.lvlManager.grid.CellToWorld(width, height) + new Vector2(offset.x, -offset.y),
            Singletons.main.lvlManager.grid.CellToWorld(-1, height) + new Vector2(-offset.x, -offset.y)
        };
        GetComponent<CinemachineConfiner>().InvalidatePathCache();
    }

    /// <summary>
    /// Reset the camera orthographic size to default value. USE WITH CAUTION!
    /// </summary>
    private void ResetSize()
    {
        vCam.m_Lens.OrthographicSize = MIN_SIZE;
    }
}