using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable, CreateAssetMenu(fileName = "New Element Rule Tile", menuName = "Tunnel/Element Rule Tile")]
public class ElementRuleTile : RuleTile<TileType>
{
    public TileType type;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is ElementRuleTile)
            return (other as ElementRuleTile).type == (TileType)neighbor;
        else
            return false;
    }
}