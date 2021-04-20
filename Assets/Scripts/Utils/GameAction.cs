﻿using UnityEngine;
using Level;

public class GameAction : MonoBehaviour
{
    public void ResetGame()
    {
        LevelManager.main.LoadLevel("1/1/1");
    }

    public void SetLevelFogVisibility(bool value)
    {
        LevelFog.main.FogIsEnabled = value;
    }
}