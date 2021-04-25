using Level;
using System.Collections.Generic;
using UnityEngine;

public class LevelPalette : MonoBehaviour
{
    [SerializeField] private List<Color> colors;
    public static System.Action<Color> Updated;

    private void Awake()
    {
        LevelManager.OnLevelPlayable += (sender, args) => Updated?.Invoke(colors[UnityEngine.Random.Range(0, colors.Count-1)]);
    }
}
