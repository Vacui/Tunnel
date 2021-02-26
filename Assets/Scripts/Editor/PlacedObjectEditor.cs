using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacedObject))]
[CanEditMultipleObjects]
public class PlacedObjectEditor : Editor {

    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;

    const int C_OpenDirectionBtnSize = 30;

    PlacedObject script;

    public override void OnInspectorGUI() {

        if (ToggleButtonStyleNormal == null) {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = EditorUtils.MakeTex(C_OpenDirectionBtnSize, 1, new Color(1.0f, 1.0f, 1.0f, 0.0f));
        }

        script = (PlacedObject)target;

        script.isSafe = EditorGUILayout.Toggle("Is Safe", script.isSafe);

        if (script.openDirectionsList != null) {
            // This code is not efficent due to the use of script.openDirectionsList.Contains, a high resource consuming method
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Button("", ToggleButtonStyleToggled, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize));
            if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-up"), script.openDirectionsList.Contains(Direction.Up) ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize))) {
                ToggleOpenDirection(Direction.Up);
            }
            GUILayout.Button("", ToggleButtonStyleToggled, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-left"), script.openDirectionsList.Contains(Direction.Left) ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize))) {
                ToggleOpenDirection(Direction.Left);
            }
            if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/x"), GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize))) {
                ClearOpenDirection();
            }
            if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-right"), script.openDirectionsList.Contains(Direction.Right) ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize))) {
                ToggleOpenDirection(Direction.Right);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Button("", ToggleButtonStyleToggled, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize));
            if (GUILayout.Button(Resources.Load<Texture>("Thumbnails/arrow-down"), script.openDirectionsList.Contains(Direction.Down) ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize))) {
                ToggleOpenDirection(Direction.Down);
            }
            GUILayout.Button("", ToggleButtonStyleToggled, GUILayout.Width(C_OpenDirectionBtnSize), GUILayout.Height(C_OpenDirectionBtnSize));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        } else {
            ClearOpenDirection();
        }

        // false to hide debug
        if (true) {
            GUILayout.Label("Open Direction List");
            if (script.openDirectionsList != null) {
                foreach (Direction dir in script.openDirectionsList) {
                    GUILayout.Label(dir.ToString());
                }
            }
        }

    }

    private void ToggleOpenDirection(Direction dir) {
        if (script.openDirectionsList.Contains(dir)) {
            script.openDirectionsList.Remove(dir);
        } else {
            script.openDirectionsList.Add(dir);
        }
    }

    private void ClearOpenDirection() {
        script.openDirectionsList = new System.Collections.Generic.List<Direction>();
    }

}