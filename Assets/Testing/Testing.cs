using UnityEngine;

public class Testing : MonoBehaviour {

    public MapGeneration mapGeneration;

    private void Awake() {
        mapGeneration.LoadMap("3/2/0-1-2-3-4-10");
    }

}