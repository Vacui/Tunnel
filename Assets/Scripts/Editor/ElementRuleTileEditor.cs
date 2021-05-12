using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ElementRuleTile), true)]
public class ElementRuleTileEditor : RuleTileEditor
{
    private const string s_Null = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAPSIixjk5Th4JIgAAAAN0Uk5TAP//RFDWIQAAACdJREFUeJxjYEADASIMDIxLMoHEypVAbtYSICE1AcoCi4FlwepQAQA4GghRf0+C6gAAAABJRU5ErkJggg==";
    private const string s_All = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAB5JREFUeJxjYEADogFAImsJAwPjqpXYCbAsRB0KAABlpgmLrVABvQAAAABJRU5ErkJggg==";
    private const string s_Up = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAC1JREFUeJxjYEAARhDB5gAkpCYAiawlQKFVK4FCq1Y5MLBGTXWASiAIVgck7QAvBghlzwdrdwAAAABJRU5ErkJggg==";
    private const string s_Right = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACtJREFUeJxjYEAAVhDB5gAiJjAwMIYtATJWrQQRqxwgLLAYWBaijhVJNwMAK6IIBbOT/U8AAAAASUVORK5CYII=";
    private const string s_Down = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAC1JREFUeJxjYEAAVgcgITUBmWCNmurAwLZqlQMD46qVQLGsJTBZNpBiRiTdDABC3ghlvZKIiAAAAABJRU5ErkJggg==";
    private const string s_Left = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACpJREFUeJxjYEAARgcgwQYipEBEVihQaNUqBwY2EAFmgcUgsmB1YB0wAAArUggFgSGXYQAAAABJRU5ErkJggg==";

    private static Texture2D[] s_Arrows;
    public static Texture2D[] elementArrows
    {
        get
        {
            if (s_Arrows == null)
                s_Arrows = new Texture2D[6]
                {
                    Base64ToTexture(s_Null),
                    Base64ToTexture(s_All),
                    Base64ToTexture(s_Up),
                    Base64ToTexture(s_Right),
                    Base64ToTexture(s_Down),
                    Base64ToTexture(s_Left)
                };

            return s_Arrows;
        }
    }

    public ElementRuleTile elementTile => target as ElementRuleTile;

    public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
    {
        if (neighbor < elementArrows.Length)
            GUI.DrawTexture(rect, elementArrows[neighbor]);
        else
            base.RuleOnGUI(rect, position, neighbor);
    }
}
