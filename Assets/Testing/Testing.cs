using UnityEngine;

public class Testing : MonoBehaviour {

    public MapGeneration mapGeneration;

    private void Awake() {
        mapGeneration.LoadMapAround("4/3/1-8-5-8-5-7-3-3-0-4-7-2", Vector3.zero);
    }

}