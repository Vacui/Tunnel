using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class GetElementVisual : MonoBehaviour
{
    [SerializeField] private ElementsVisuals visuals;
    [SerializeField] private TileType tileType;

    private void Awake()
    {
        GetElementVisualToSprite();
    }

#if (UNITY_EDITOR)
    private void Update()
    {
        GetElementVisualToSprite();
    }
#endif

    private void GetElementVisualToSprite()
    {
        if (visuals != null) GetComponent<Image>().sprite = visuals.GetVisual(tileType);
    }
}