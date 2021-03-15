﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class MyUtils
{
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

    public static void ClearLogConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
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

public enum Direction
{
    NULL,
    Up,
    Down,
    Right,
    Left
}

public static class DirectionUtils
{
    public static Direction Opposite(this Direction dir)
    {
        switch (dir)
        {
            default:
            case Direction.Up: return Direction.Down;
            case Direction.Right: return Direction.Left;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
        }
    }

    public static Vector3Int ToOffset(this Direction dir)
    {
        Vector3Int result = Vector3Int.zero;
        switch (dir)
        {
            case Direction.Up: result.z++; break;
            case Direction.Right: result.x++; break;
            case Direction.Down: result.z--; break;
            case Direction.Left: result.x--; break;
        }
        return result;
    }

    public static float ToAngle(this Direction dir)
    {
        float result = -1;

        if (dir != Direction.NULL)
        {
            result = (int)dir * 90;
        }

        return result;
    }
}

[System.Serializable]
public enum TileType
{
    NULL,
    Start,
    End,
    Node,
    FacingUp,
    FacingRight,
    FacingDown,
    FacingLeft,
    Intersection 
}

public static class TileTypeUtils
{
    public static string TileTypeArrayToString(TileType[] array)
    {
        string result = "";
        for (int i = 0; i < array.Length; i++)
        {
            result += $"-{array[i]}";
        }
        result = result.Trim('-');
        return result;
    }
}