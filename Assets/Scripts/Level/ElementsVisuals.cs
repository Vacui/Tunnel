using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Elements Visuals", menuName = "Tunnel/Elements Visuals")]
public class ElementsVisuals : ScriptableObject
{
    [System.Serializable]
    public class ElementVisual
    {
        public TileType type;
        public Sprite visual;
    }

    [SerializeField] private Sprite defaultVisual;
    [SerializeField, ReorderableList] private ElementVisual[] arrayElementSkins;
    private Dictionary<TileType, Sprite> dictionaryTileSkins;

    private void GenerateDictionary()
    {
        dictionaryTileSkins = new Dictionary<TileType, Sprite>();
        dictionaryTileSkins.Add(TileType.NULL, defaultVisual); ;
        ElementVisual tileSkin;
        for (int i = 0; i < arrayElementSkins.Length; i++)
        {
            tileSkin = arrayElementSkins[i];
            if (!dictionaryTileSkins.ContainsKey(tileSkin.type))
                dictionaryTileSkins.Add(tileSkin.type, tileSkin.visual);
        }
    }

    public Sprite GetVisual(TileType type)
    {
        if (dictionaryTileSkins == null)
            GenerateDictionary();

        Sprite result = dictionaryTileSkins[TileType.NULL];

        if (dictionaryTileSkins.ContainsKey(type))
            result = dictionaryTileSkins[type];

        return result;
    }
}