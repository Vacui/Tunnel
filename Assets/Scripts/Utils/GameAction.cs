using UnityEngine;

public class GameAction : MonoBehaviour
{
    public void ResetGame()
    {
        Singletons.main.lvlManager.LoadLevel(new Level.LevelManager.Seed("1/1/1"));
    }

    public void SetLevelFogVisibility(bool value)
    {
        Singletons.main.lvlFog.FogIsEnabled = value;
    }
}