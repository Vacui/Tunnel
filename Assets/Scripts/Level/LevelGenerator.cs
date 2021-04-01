using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public void GenerateLevel(int width, int height)
    {
        Debug.Log($"Generating level {width}x{height}");
        string seed = $"{width}/{height}/1-";
        for (int i = 1; i < width * height; i++)
            seed += "2-";

        Singletons.main.lvlFog.hideLevel = false;
        Singletons.main.lvlManager.LoadLevel(new LevelManager.Seed(seed.Trim('-')));
    }
}