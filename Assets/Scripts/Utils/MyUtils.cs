using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class MyUtils
{
    // origin: https://www.codegrepper.com/code-examples/csharp/how+to+clear+console+through+script+unity
    public static void ClearLogConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }

    public static void AddBlankRange(this List<string> list, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add("");
            }
        }
    }

    public static void SetObjectsActive(GameObject[] gameObjects, bool active)
    {
        if (gameObjects != null && gameObjects.Length > 0)
            foreach (GameObject gameObject in gameObjects)
                SetObjectActive(gameObject, active);
    }

    public static void SetObjectActive(GameObject gameObject, bool active) { if (gameObject != null) gameObject?.SetActive(active); }

    public static List<Vector2Int> GatherNeighbours(int x = 0, int y = 0, int radius = 1, bool avoidCenter = false, bool avoidCorners = false)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        if (radius > 0)
            for (int xT = -radius; xT < radius + 1; xT++)
                for (int yT = -radius; yT < radius + 1; yT++)
                    if (!avoidCenter || (xT != 0 || yT != 0))
                        if (!avoidCorners || (Mathf.Abs(xT) != Mathf.Abs(yT)))
                            neighbours.Add(new Vector2Int(x + xT, y + yT));
        return neighbours;
    }

    public static int RandomWithExceptions(int start, int end, List<int> exceptions)
    {
        bool ok = false;
        int result = 0;

        if (exceptions != null)
        {

            int limit = Mathf.Max(start, end) - Mathf.Min(start, end);
            int tests = 0;
            while (!ok && tests < limit)
            {
                result = UnityEngine.Random.Range(start, end);
                ok = !exceptions.Contains(result);
                tests++;
            }
        } else
        {
            ok = true;
            result = UnityEngine.Random.Range(start, end);
        }

        if (ok) return result;
        else throw new KeyNotFoundException();
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        return worldCamera.ScreenToWorldPoint(screenPosition);
    }
    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ();
        vec.z = 0f;
        return vec;
    }
}

public static class ListUtils
{
    public static void RemoveLast<T>(this List<T> list)
    {
        if (list != null && list.Count > 0)
            list.RemoveAt(list.Count - 1);
    }

    public static T Last<T>(this List<T> list, int offset = 0)
    {
        T result = default;

        if (list != null && list.Count > 0)
        {
            offset = Mathf.Clamp(offset, 0, list.Count - 1);


            if (list.Count > 0)
                result = list[list.Count - 1 - offset];
        }

        return result;
    }
}

public static class UIUtils
{
    public static TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, Quaternion localRotation, Color fontColor, int fontSize = 40, TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
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
    public static Font GetDefaultFont()
    {
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}

public static class EditorUtils
{
    // Method made by Andeeeee
    // Source: https://forum.unity.com/threads/giving-unitygui-elements-a-background-color.20510/#post-422235
    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}