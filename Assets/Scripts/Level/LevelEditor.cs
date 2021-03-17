using System;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [System.Serializable]
    public enum State
    {
        NotReady,
        NotPlayable,
        Playable,
        Ready
    }

    public static event Action OnLevelNotReady;
    public static event Action OnLevelNotPlayable;
    public static event Action OnLevelPlayable;
    public static event Action OnLevelReady;

    [SerializeField] LevelManager levelManager;

    private void Start()
    {
        OnLevelNotReady?.Invoke();
    }

    private void CreateLevelBlank()
    {
        CreateLevel(new Vector2Int(1, 1));
    }
    public void CreateLevel(Vector2Int size)
    {
        if(size.x > 0 && size.y > 0)
        {
            levelManager.InitializeLevel(size.x, size.y);
            OnLevelNotPlayable?.Invoke();
        }
    }
}