using UnityEngine;

public class Testing : MonoBehaviour {

    public MapGeneration mapGeneration;
    public string testSeed;

    private void Awake() {
        mapGeneration.LoadMap(testSeed);
    }

}