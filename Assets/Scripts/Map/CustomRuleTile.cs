using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(fileName = "New Custom Rule Tile", menuName = "2D Extras/Tiles/Custom Rule Tile", order = 359)]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor>
{
    public TileType tileType;
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Start = 3;
        public const int End = 4;
        public const int Node = 5;
        public const int FacingUp = 6;
        public const int FacingRight = 7;
        public const int FacingDown = 8;
        public const int FacingLeft = 9;
    }

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (neighbor > 2)
        {
            CustomRuleTile otherCustomRuleTile = other as CustomRuleTile;
            if (otherCustomRuleTile)
            {
                Debug.Log($"neighbor={neighbor}({(TileType)(neighbor - 2)})"
                    + $" ---- other={otherCustomRuleTile.tileType}"
                    + $" ---- this={tileType}.");
                return otherCustomRuleTile.tileType == (TileType)(neighbor - 2);
            }
            return false;
        }
        return base.RuleMatch(neighbor, other);
    }
}