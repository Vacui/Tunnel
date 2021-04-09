using UnityEngine;

public class SetInactive : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void SetObjInactive(bool value)
    {
        if (target != null) target.SetActive(!value);
    }
}
