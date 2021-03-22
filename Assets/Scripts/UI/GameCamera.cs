using System.Collections;
using System.Collections.Generic;
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
        if (height > width)
            gameCamera.orthographicSize = height;
        else
            gameCamera.orthographicSize = width + 2;
    }
}