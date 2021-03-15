using TMPro;
using UnityEngine;

public class LevelTemplateElement : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    public void GetSize(out int width, out int height)
    {
        width = this.width;
        height = this.height;
    }
}
