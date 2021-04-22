using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Elements Visuals", menuName = "Tunnel/Elements Visuals")]
public class ElementsVisuals : ScriptableObject
{
    [System.Serializable]
    public class Visual
    {
        public TileType type;
        public TileBase tileBase;
    }

    [SerializeField] private TileBase defaultVisual;
    [SerializeField, ReorderableList] private Visual[] arrayElementSkins;
    private Dictionary<TileType, TileBase> dictionaryTileSkins;

    private void GenerateDictionary()
    {
        dictionaryTileSkins = new Dictionary<TileType, TileBase>();
        dictionaryTileSkins.Add(TileType.NULL, defaultVisual); ;
        Visual tileSkin;
        for (int i = 0; i < arrayElementSkins.Length; i++)
        {
            tileSkin = arrayElementSkins[i];
            if (!dictionaryTileSkins.ContainsKey(tileSkin.type))
                dictionaryTileSkins.Add(tileSkin.type, tileSkin.tileBase);
        }
    }

    public TileBase GetTileBase(TileType type)
    {
        TileBase result = null;

        if (dictionaryTileSkins == null)
            GenerateDictionary();
        
        if (dictionaryTileSkins.ContainsKey(type))
            result = dictionaryTileSkins[type];
        else
            result = dictionaryTileSkins[TileType.NULL];

        return result;
    }
}