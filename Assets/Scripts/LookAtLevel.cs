using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LookAtLevel : MonoBehaviour {
    private const float MAX_SIZE = 14f;
    private const float MIN_SIZE = 7.7f;

    private Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void OnEnable() {
        Level.LevelManager.OnLevelReady += (sender, args) => ResizeCamera(args.width, args.height);
    }

    private void ResizeCamera(int width, int height) {
        cam.orthographicSize = width * 1.3f;
        cam.transform.position = new Vector3((width - 1) / 2.0f, (height - 1) / 2.0f, cam.transform.position.z);
    }
}