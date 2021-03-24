using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Skin", menuName = "Tunnel/Level Skin")]
public class LevelSkin : ScriptableObject
{
    [System.Serializable]
    public class TileSkin
    {
        public TileType type;
        public Sprite empty;
        public Sprite full;
    }

    [SerializeField] private TileSkin defaultSkin;
    [SerializeField] private TileSkin unknownSkin;
    [SerializeField, ReorderableList] private TileSkin[] arrayTileSkins;
    private Dictionary<TileType, TileSkin> dictionaryTileSkins;

    private void GenerateDictionary()
    {
        dictionaryTileSkins = new Dictionary<TileType, TileSkin>();
        TileSkin tileSkin;
        for (int i = 0; i < arrayTileSkins.Length; i++)
        {
            tileSkin = arrayTileSkins[i];
            if (tileSkin != null && tileSkin.type != TileType.NULL && tileSkin.type != TileType.Player)
            {
                if (!dictionaryTileSkins.ContainsKey(tileSkin.type))
                    dictionaryTileSkins.Add(tileSkin.type, tileSkin);
            }
        }
    }

    public TileSkin GetSkin(TileType type)
    {
        TileSkin result = defaultSkin;

        if (dictionaryTileSkins == null)
            GenerateDictionary();

        if (dictionaryTileSkins.ContainsKey(type))
            result = dictionaryTileSkins[type];

        return result;
    }

    public TileSkin GetUnknownSkin()
    {
        return unknownSkin;
    }
}