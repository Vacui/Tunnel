using UnityEngine;

public class SetGameObjectInactive : MonoBehaviour {
    [SerializeField] private GameObject target;

    public void SetInactive(bool value) {
        if (target != null) {
            target.SetActive(!value);
        }
    }
}