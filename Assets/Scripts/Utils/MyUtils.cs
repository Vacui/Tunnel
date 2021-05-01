﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class MyUtils {
    // source: https://www.codegrepper.com/code-examples/csharp/how+to+clear+console+through+script+unity
    public static void ClearLogConsole() {
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }

    //source: https://stackoverflow.com/a/5320727
    public static bool In<T>(this T val, params T[] values) where T : struct {
        return values.Contains(val);
    }

    public static void AddBlankRange(this List<string> list, int count) {
        if (count <= 0) return;

        for (int i = 0; i < count; i++) {
            list.Add("");
        }
    }

    public static void SetObjectsActive(GameObject[] gameObjects, bool active) {
        if (gameObjects == null || gameObjects.Length <= 0) return;

        foreach (GameObject gameObject in gameObjects) {
            SetObjectActive(gameObject, active);
        }
    }

    public static void SetObjectActive(GameObject gameObject, bool active) {
        if (gameObject == null) return;

        gameObject?.SetActive(active);
    }

    public static List<Vector2Int> GatherNeighbours(int x = 0, int y = 0, int radius = 1, bool avoidCenter = false, bool avoidCorners = false) {
        if (radius <= 0) return null;

        List<Vector2Int> neighbours = new List<Vector2Int>();
        for (int xT = -radius; xT < radius + 1; xT++) {
            for (int yT = -radius; yT < radius + 1; yT++) {
                if (!avoidCenter || (xT != 0 || yT != 0)) {
                    if (!avoidCorners || (Mathf.Abs(xT) != Mathf.Abs(yT))) {
                        neighbours.Add(new Vector2Int(x + xT, y + yT));
                    }
                }
            }
        }
        return neighbours;
    }

    public static int RandomWithExceptions(int start, int end, List<int> exceptions) {
        if (exceptions != null) {
            int result;
            int limit = Mathf.Max(start, end) - Mathf.Min(start, end);
            int tests = 0;
            bool tryAgain = true;
            while (tests < limit && tryAgain) {
                result = UnityEngine.Random.Range(start, end);
                if (!exceptions.Contains(result)) return result;
                tests++;
            }
            throw new KeyNotFoundException();
        } else {
            return UnityEngine.Random.Range(start, end);
        }
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
        return worldCamera.ScreenToWorldPoint(screenPosition);
    }
    public static Vector3 GetMouseWorldPositionWithZ() {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPosition() {
        Vector3 vec = GetMouseWorldPositionWithZ();
        vec.z = 0f;
        return vec;
    }
}

public static class GameDebug {

    private static string ApplyMessageFormat(object message, UnityEngine.Object context = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0) {
        return string.Format("[{0}:{1}] ({2}) {3}", memberName, sourceLineNumber, sourceFilePath.Substring(sourceFilePath.LastIndexOf(@"\") + 1), message);
    }

    public static void Log(object message, UnityEngine.Object context = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) {
        message = ApplyMessageFormat(message, context, memberName, sourceFilePath, sourceLineNumber);
        Debug.Log(message, context);
    }

    public static void LogWarning(object message, UnityEngine.Object context = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) {
        message = ApplyMessageFormat(message, context, memberName, sourceFilePath, sourceLineNumber);
        Debug.LogWarning(message, context);
    }

    public static void LogError(object message, UnityEngine.Object context = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) {
        message = ApplyMessageFormat(message, context, memberName, sourceFilePath, sourceLineNumber);
        Debug.LogError(message, context);
    }

}

public static class ListUtils {
    public static void RemoveLast<T>(this List<T> list, int offset = 0) {
        if (list == null || list.Count <= 0) {
            return;
        }

        list.RemoveAt(Mathf.Clamp(list.Count - 1 - offset, 0, list.Count - 1));
    }

    public static T Last<T>(this List<T> list, int offset = 0) {
        if (list == null || list.Count == 0) {
            return default;
        }

        offset = Mathf.Clamp(offset, 0, list.Count - 1);
        return list[list.Count - 1 - offset];
    }
}

public static class UIUtils {
    public static TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, Quaternion localRotation, Color fontColor, int fontSize = 40, TextAnchor textAnchor = TextAnchor.MiddleCenter) {
        if (fontColor == null) fontColor = Color.white;
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.fontSize = fontSize;
        textMesh.color = fontColor;
        textMesh.font = GetDefaultFont();
        textMesh.text = text;
        return textMesh;
    }

    // Get Default Unity Font, used in text objects if no font given
    public static Font GetDefaultFont() {
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}

public static class EditorUtils {
    // Method made by Andeeeee
    // Source: https://forum.unity.com/threads/giving-unitygui-elements-a-background-color.20510/#post-422235
    public static Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++) {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}