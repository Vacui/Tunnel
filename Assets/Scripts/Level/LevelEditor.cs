using System;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;

    private void CreateLevelBlank()
    {
        CreateLevel(new Vector2Int(1, 1));
    }
    public void CreateLevel(Vector2Int size)
    {
        if(size.x > 0 && size.y > 0)
        {
            levelManager.InitializeLevel(size.x, size.y);
        }
    }
}