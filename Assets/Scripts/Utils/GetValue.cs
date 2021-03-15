using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class GetValue : MonoBehaviour
{
    public void GetValueToString(float value)
    {
        GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
}