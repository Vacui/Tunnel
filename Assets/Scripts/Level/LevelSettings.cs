using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    const int DEFAULT_WIDTH = 5;
    const int DEFAULT_HEIGHT = 5;
    [SerializeField, Disable] int lvlWidth;
    [SerializeField, Disable] int lvlHeight;

    private void Awake()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        lvlWidth = DEFAULT_WIDTH;
        lvlHeight = DEFAULT_HEIGHT;
    }

    public void SetWidth(float value) { lvlWidth = Mathf.RoundToInt(value); }
    public void SetHeight(float value) { lvlHeight = Mathf.RoundToInt(value); }

    public void ApplySettings()
    {
        Singletons.main.lvlGenerator.GenerateLevel(lvlWidth, lvlHeight);
    }
}