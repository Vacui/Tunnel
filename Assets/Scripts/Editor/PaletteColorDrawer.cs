using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Palette.PaletteColor))]
public class PaletteColorDrawer : PropertyDrawer {

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        float border = 5f;
        float width = position.width / 3f;
        Rect primaryRect = new Rect(position.x, position.y, width, position.height);
        position.x += width + border;
        Rect secondaryRect = new Rect(position.x, position.y, width, position.height);
        position.x += width + border;
        Rect textRect = new Rect(position.x, position.y, position.width - ((width + border) * 2f), position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(primaryRect, property.FindPropertyRelative("primary"), GUIContent.none);
        EditorGUI.PropertyField(secondaryRect, property.FindPropertyRelative("secondary"), GUIContent.none);
        EditorGUI.PropertyField(textRect, property.FindPropertyRelative("text"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}