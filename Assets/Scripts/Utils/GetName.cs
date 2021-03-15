using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class GetName : MonoBehaviour
{
    [SerializeField] private Transform parent;

#if (UNITY_EDITOR)
    private void Awake()
    {
        parent = transform.parent;
    }

    private void Update()
    {
        GetParentName();
    }

    private void GetParentName()
    {
        if (parent != null) GetComponent<TextMeshProUGUI>().text = parent.name;
    }
#endif
}