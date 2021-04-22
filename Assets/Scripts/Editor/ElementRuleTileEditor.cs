using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ElementRuleTile), true)]
public class ElementRuleTileEditor : RuleTileEditor
{
    private const string s_Blank = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACdJREFUeJxjYEAHIiAiE4gZVwIJtiVAQmoCkMhygAoxgoQYAjA0AgCqlwTF0yShrgAAAABJRU5ErkJggg==";
    private const string s_Start = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAPiMjxjk5TVY1WQAAAAN0Uk5TAP//RFDWIQAAAB5JREFUeJxjYEADogFAImsJAwPjqpXYCbAsRB0KAABlpgmLrVABvQAAAABJRU5ErkJggg==";
    private const string s_End = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIz8nNrxIDF6XvAAAAAN0Uk5TAP//RFDWIQAAAB5JREFUeJxjYEADogFAImsJAwPjqpXYCbAsRB0KAABlpgmLrVABvQAAAABJRU5ErkJggg==";
    private const string s_Node = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAALi4ugICAt9fEBQAAAAN0Uk5TAP//RFDWIQAAAB5JREFUeJxjYEADogFAImsJAwPjqpXYCbAsRB0KAABlpgmLrVABvQAAAABJRU5ErkJggg==";
    private const string s_ArrowUp = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAC1JREFUeJxjYEAARhDB5gAkpCYAiawlQKFVK4FCq1Y5MLBGTXWASiAIVgck7QAvBghlzwdrdwAAAABJRU5ErkJggg==";
    private const string s_ArrowRight = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACtJREFUeJxjYEAAVhDB5gAiJjAwMIYtATJWrQQRqxwgLLAYWBaijhVJNwMAK6IIBbOT/U8AAAAASUVORK5CYII=";
    private const string s_ArrowDown = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAC1JREFUeJxjYEAAVgcgITUBmWCNmurAwLZqlQMD46qVQLGsJTBZNpBiRiTdDABC3ghlvZKIiAAAAABJRU5ErkJggg==";
    private const string s_ArrowLeft = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACpJREFUeJxjYEAARgcgwQYipEBEVihQaNUqBwY2EAFmgcUgsmB1YB0wAAArUggFgSGXYQAAAABJRU5ErkJggg==";

    private static Texture2D[] s_Arrows;
    public static Texture2D[] elementArrows
    {
        get
        {
            if (s_Arrows == null)
                s_Arrows = new Texture2D[8]
                {
                    Base64ToTexture(s_Blank),
                    Base64ToTexture(s_Start),
                    Base64ToTexture(s_Node),
                    Base64ToTexture(s_End),
                    Base64ToTexture(s_ArrowUp),
                    Base64ToTexture(s_ArrowRight),
                    Base64ToTexture(s_ArrowDown),
                    Base64ToTexture(s_ArrowLeft)
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
