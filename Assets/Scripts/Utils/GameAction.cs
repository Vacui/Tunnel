﻿using UnityEngine;

public class GameAction : MonoBehaviour
{
    public void ResetGame()
    {
        Singletons.main.lvlManager.LoadLevel(new LevelManager.Seed("1/1/1"));
    }

    public void ToggleFog(bool value)
    {
        Singletons.main.lvlFog.SetFog(value);
    }
}