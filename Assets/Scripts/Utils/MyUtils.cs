using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

public static class MyUtils {
    // source: https://www.codegrepper.com/code-examples/csharp/how+to+clear+console+through+script+unity
    // Commented to avoid build error
    public static void ClearLogConsole() {
        //Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        //Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        //MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        //clearConsoleMethod.Invoke(new object(), null);
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

    // source: https://stackoverflow.com/a/129395
    public static T DeepClone<T>(T obj) {
        using (var ms = new System.IO.MemoryStream()) {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    // source: https://unitycodemonkey.com/utils.php
    public static float GetAngleFromVectorFloat(Vector3 dir) {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 RemoveZ(this Vector3 vector) {
        return new Vector3(vector.x, vector.y, 0f);
    }

    public static Color SetColorAlpha(Color color, float alpha) {
        color.a = alpha;
        return color;
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

    // source: https://stackoverflow.com/a/1262619
    public static void ShuffleUsingRandom<T>(this IList<T> list) {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // source: https://stackoverflow.com/a/1262619
    public static void ShuffleUsingCryptography<T>(this IList<T> list) {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1) {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void ShuffleUsingLinq<T>(this IList<T> list) {
        System.Random rng = new System.Random();
        list = list.OrderBy(a => rng.Next()).ToList();
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

    // source: https://forum.unity.com/threads/layoutgroup-does-not-refresh-in-its-current-frame.458446/#post-6712318
    public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject root) {
        LayoutGroup[] layoutGroupsInChildren = root.GetComponentsInChildren<LayoutGroup>(true);

        foreach (LayoutGroup layoutGroup in layoutGroupsInChildren) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }

        LayoutGroup parentLayoutGroup = root.GetComponent<LayoutGroup>();

        if (parentLayoutGroup != null) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroup.GetComponent<RectTransform>());
        }
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

[Serializable]
public sealed class UltEventColor : UltEvent<Color> { }

[Serializable]
public sealed class UltEventString : UltEvent<string> { }

[Serializable]
public sealed class UltEventFloat : UltEvent<float> { }

[Serializable]
public sealed class UltEventVector3 : UltEvent<Vector3> { }