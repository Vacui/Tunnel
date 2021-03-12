using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

}

//[CustomPropertyDrawer(typeof(DirectionsList))]
public class DirectionsListDrawer : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight * 5;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        var up = property.FindPropertyRelative("up");
        var right = property.FindPropertyRelative("right");
        var down = property.FindPropertyRelative("down");
        var left = property.FindPropertyRelative("left");

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);

        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(new Rect(position.x, position.y + 18 * 1, position.width, EditorGUIUtility.singleLineHeight), up);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 18 * 2, position.width, EditorGUIUtility.singleLineHeight), right);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 18 * 3, position.width, EditorGUIUtility.singleLineHeight), down);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 18 * 4, position.width, EditorGUIUtility.singleLineHeight), left);

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

}