using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomRuleTile), true)]
[CanEditMultipleObjects]
public class CustomRuleTileEditor : RuleTileEditor
{
    private const string s_Start = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";
    private const string s_End = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";
    private const string s_Node = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";
    private const string s_ArrowUp = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAC1JREFUeJxjYEAARhDB5gAkpCYAiawlQKFVK4FCq1Y5MLBGTXWASiAIVgck7QAvBghlzwdrdwAAAABJRU5ErkJggg==";
    private const string s_ArrowRight = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACtJREFUeJxjYEAAVhDB5gAiJjAwMIYtATJWrQQRqxwgLLAYWBaijhVJNwMAK6IIBbOT/U8AAAAASUVORK5CYII=";
    private const string s_ArrowDown = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAAC1JREFUeJxjYEAAVgcgITUBmWCNmurAwLZqlQMD46qVQLGsJTBZNpBiRiTdDABC3ghlvZKIiAAAAABJRU5ErkJggg==";
    private const string s_ArrowLeft = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACpJREFUeJxjYEAARgcgwQYipEBEVihQaNUqBwY2EAFmgcUgsmB1YB0wAAArUggFgSGXYQAAAABJRU5ErkJggg==";
    private const string s_Intersection = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPAgMAAABGuH3ZAAAAAXNSR0IB2cksfwAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAlQTFRFAAAAIzc+OYDGY5BUjQAAAAN0Uk5TAP//RFDWIQAAACVJREFUeJxjYEAAVgcgITUBSjBGTWVgYFu1ygGJAIshlEB0wAAAcEYJ4bSCm7YAAAAASUVORK5CYII=";

    private static Texture2D[] s_Arrows;
    public static Texture2D[] customArrows
    {
        get
        {
            if (s_Arrows == null)
            {
                s_Arrows = new Texture2D[11];
                s_Arrows[3] = Base64ToTexture(s_Start);
                s_Arrows[4] = Base64ToTexture(s_End);
                s_Arrows[5] = Base64ToTexture(s_Node);
                s_Arrows[6] = Base64ToTexture(s_ArrowUp);
                s_Arrows[7] = Base64ToTexture(s_ArrowRight);
                s_Arrows[8] = Base64ToTexture(s_ArrowDown);
                s_Arrows[9] = Base64ToTexture(s_ArrowLeft);
                s_Arrows[10] = Base64ToTexture(s_Intersection);
            }
            return s_Arrows;
        }
    }

    public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
    {
        if(neighbor > 2)
        {
            GUI.DrawTexture(rect, customArrows[neighbor]);
        } else
        {
            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}