using Malee.List;
using UnityEngine;

namespace Tunnel
{

    [CreateAssetMenu(fileName = "New RuleTileVisual", menuName = "RuleTileVisual")]
    public class RuleTileVisual : ScriptableObject
    {
        [SerializeField] private GameObject defaultModel;
        [SerializeField] [Reorderable] private TilingVisualRules tilingVisualRules;

        public GameObject GetVisual(TileType[] neighbours)
        {
            GameObject model = null;
            if (tilingVisualRules != null)
            {
                for (int i = 0; i < tilingVisualRules.Count && model == null; i++)
                {
                    model = tilingVisualRules[i].Match(neighbours);
                }
            }
            return model ? model : defaultModel;
        }

    }

    [System.Serializable]
    public class TilingVisualRule
    {
        [SerializeField] private string name;
        [SerializeField] private TileType[] match = new TileType[5];
        [SerializeField] private GameObject visualModel;

        public GameObject Match(TileType[] match)
        {
            bool result = false;
            if (this.match != null)
            {
                if (match.Length == 5)
                {
                    if (this.match.Length == match.Length)
                    {
                        result = true;
                        for (int i = 0; i < match.Length; i++)
                        {
                            result = result && (this.match[i] == TileType.NULL || this.match[i] == match[i]);
                        }
                        //Debug.Log($"Match {result} for {TileTypeUtils.TileTypeArrayToString(match)} with {TileTypeUtils.TileTypeArrayToString(this.match)}.");
                    }
                }
            }

            return result ? visualModel : null;
        }
    }

    [System.Serializable]
    public class TilingVisualRules : ReorderableArray<TilingVisualRule> { }
}