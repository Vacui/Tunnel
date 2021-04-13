using Level;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public class LoadSeed : MonoBehaviour
    {
        [SerializeField] private string seed;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Load);
        }

        private void Load()
        {
            Singletons.main.lvlManager.LoadLevel(new LevelManager.Seed(seed));
        }
    }
}