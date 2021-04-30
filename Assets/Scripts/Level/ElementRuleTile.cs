using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Level;

[Serializable, CreateAssetMenu(fileName = "New Element Rule Tile", menuName = "Tunnel/Element Rule Tile")]
public class ElementRuleTile : RuleTile<Direction>
{
    public Direction baseDir = Direction.Up;

    public override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
    {
        if (RuleMatches(rule, position, tilemap, 0))
        {
            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            return true;
        }

        if (rule.m_RuleTransform == TilingRule.Transform.Rotated)
        {
            for (int angle = m_RotationAngle; angle < 360; angle += m_RotationAngle)
            {
                if (RuleMatches(rule, position, tilemap, angle))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
                    return true;
                }
            }
        }

        return false;
    }

    public new bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle)
    {
        try
        {
            Direction gridDirection = LevelManager.main.Grid.GetTile(position.x, position.y).ToDirection();
            Direction rotatedDirection = baseDir.Rotate(angle);
            if (gridDirection == rotatedDirection)
            {
                var minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
                for (int i = 0; i < minCount; i++)
                {
                    int neighbor = rule.m_Neighbors[i];
                    Vector3Int positionOffset = GetRotatedPosition(rule.m_NeighborPositions[i], angle);
                    Vector3Int offsetPosition = GetOffsetPosition(position, positionOffset);
                    TileBase other = tilemap.GetTile(offsetPosition);
                    if (!RuleMatch(offsetPosition, (int)((Direction)neighbor).Rotate(angle), other))
                        return false;
                }
                return true;
            } else return false;
        } catch (Exception)
        {
            return false;
        }
    }

    public bool RuleMatch(Vector3Int position, int neighbor, TileBase other)
    {
        try
        {
            Direction gridNeighborDirection = LevelManager.main.Grid.GetTile(position.x, position.y).ToDirection();
            return gridNeighborDirection == Direction.All || gridNeighborDirection == (Direction)neighbor;
        } catch (Exception)
        {
            return false;
        }
    }
}