/* based on tutorial made by Thousand Ant
 * link: https://www.youtube.com/watch?v=tcatvGLvCDc
 * */

using UnityEngine;

public class Singletons : MonoBehaviour
{
    public static Singletons main { get; private set; }

    public LevelManager lvlManager { get; private set; }
    public LevelVisual lvlVisual { get; private set; }
    public LevelFog lvlFog { get; private set; }
    public LevelGenerator lvlGenerator { get; private set; }
    public UIManager uiManager { get; private set; }
    public Player player { get; private set; }
    public GameCamera gameCamera { get; private set; }

    private void Awake()
    {
        if (main != null)
        {
            Destroy(gameObject);
            return;
        } else
            main = this;

        LeanTween.init(2000);

        lvlManager = GetComponentInChildren<LevelManager>();
        lvlVisual = GetComponentInChildren<LevelVisual>();
        lvlFog = GetComponentInChildren<LevelFog>();
        lvlGenerator = GetComponentInChildren<LevelGenerator>();
        uiManager = GetComponentInChildren<UIManager>();
        player = GetComponentInChildren<Player>();
        gameCamera = FindObjectOfType<GameCamera>();
    }
}