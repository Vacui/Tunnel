using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacedObject))]
public class PlacedObjectEditor : Editor {

    public override void OnInspectorGUI() {

        Color defaultColor = GUI.color;

        PlacedObject script = (PlacedObject)target;

        script.isSafe = EditorGUILayout.Toggle("Is Safe", script.isSafe);

        // This code is not efficent due to the use of script.openDirectionsList.Contains, a high resource consuming method

        GUILayout.BeginHorizontal();

        GUI.color = Color.red;
        if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/x"), GUILayout.Width(30), GUILayout.Height(30))) {
            script.ClearOpenDirection();
        }

        GUI.color = script.openDirectionsList.Contains(Direction.Up) ? Color.green : defaultColor;
        if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-up"), GUILayout.Width(30), GUILayout.Height(30))) {
            script.ToggleOpenDirection(Direction.Up);
        }

        GUI.color = script.openDirectionsList.Contains(Direction.Right) ? Color.green : defaultColor;
        if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-right"), GUILayout.Width(30), GUILayout.Height(30))) {
            script.ToggleOpenDirection(Direction.Right);
        }

        GUI.color = script.openDirectionsList.Contains(Direction.Down) ? Color.green : defaultColor;
        if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-down"), GUILayout.Width(30), GUILayout.Height(30))) {
            script.ToggleOpenDirection(Direction.Down);
        }

        GUI.color = script.openDirectionsList.Contains(Direction.Left) ? Color.green : defaultColor;
        if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-left"), GUILayout.Width(30), GUILayout.Height(30))) {
            script.ToggleOpenDirection(Direction.Left);
        }

        GUILayout.EndHorizontal();

    }

}