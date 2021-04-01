using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSeed : MonoBehaviour
{
    [SerializeField] private string seed;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Load);
    }

    private void Load()
    {
        Singletons.main.lvlFog.hideLevel = true;
        Singletons.main.lvlManager.LoadLevel(new LevelManager.Seed(seed));
    }
}