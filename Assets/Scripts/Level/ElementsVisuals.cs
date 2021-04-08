using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Elements Visuals", menuName = "Tunnel/Elements Visuals")]
public class ElementsVisuals : ScriptableObject
{
    [System.Serializable]
    public class Visual
    {
        public TileType type;
        public VisualData data;
    }

    [System.Serializable]
    public class VisualData
    {
        public VisualData(Sprite sprite, Color color)
        {
            this.sprite = sprite;
            this.color = color;
        }

        public Sprite sprite;
        public Color color;
    }

    [SerializeField] private VisualData defaultVisual;
    [SerializeField, ReorderableList] private Visual[] arrayElementSkins;
    private Dictionary<TileType, VisualData> dictionaryTileSkins;

    private void GenerateDictionary()
    {
        dictionaryTileSkins = new Dictionary<TileType, VisualData>();
        dictionaryTileSkins.Add(TileType.NULL, defaultVisual); ;
        Visual tileSkin;
        for (int i = 0; i < arrayElementSkins.Length; i++)
        {
            tileSkin = arrayElementSkins[i];
            if (!dictionaryTileSkins.ContainsKey(tileSkin.type))
                dictionaryTileSkins.Add(tileSkin.type, tileSkin.data);
        }
    }

    public VisualData GetVisualData(TileType type)
    {
        VisualData result = null;

        if (dictionaryTileSkins == null)
            GenerateDictionary();
        
        if (dictionaryTileSkins.ContainsKey(type))
            result = dictionaryTileSkins[type];
        else
            result = dictionaryTileSkins[TileType.NULL];

        return result;
    }
}