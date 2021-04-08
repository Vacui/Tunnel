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
        GetElementVisualSprite();
    }

#if (UNITY_EDITOR)
    private void Update()
    {
        GetElementVisualSprite();
    }
#endif

    private void GetElementVisualSprite()
    {
        if (visuals != null) GetComponent<Image>().sprite = visuals.GetVisualData(tileType).sprite;
    }
}