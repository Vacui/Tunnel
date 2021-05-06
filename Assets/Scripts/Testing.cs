using Unity.Mathematics;
using UnityEngine;

public class Testing : MonoBehaviour {

    private void Awake() {
        Debug.Log(new Vector3(1, 1, 1) + (Vector3)new float3(1, 1, 1));
    }
}