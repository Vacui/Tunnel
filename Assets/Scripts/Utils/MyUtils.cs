﻿using System.Collections.Generic;
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

    public static void SetObjectsActive(GameObject[] gameObjects, bool active)
    {
        if (gameObjects != null && gameObjects.Length > 0)
            foreach (GameObject gameObject in gameObjects)
                if (gameObject)
                    gameObject?.SetActive(active);
    }

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
}

public static class ListUtils
{
    public static void RemoveLast<T>(this List<T> list)
    {
        if (list != null && list.Count > 0)
            list.RemoveAt(list.Count - 1);
    }

    public static T Last<T>(this List<T> list)
    {
        T result = default(T);

        if (list.Count > 0)
            result = list[list.Count - 1];

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

public enum Direction
{
    NULL,
    All,
    Up,
    Right,
    Down,
    Left
}

public static class DirectionUtils
{
    public static bool IsNull(this Direction dir)
    {
        return dir == Direction.NULL;
    }

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

    public static void ToOffset(this Direction dir, out int offsetX, out int offsetY)
    {
        offsetX = 0;
        offsetY = 0;
        switch (dir)
        {
            case Direction.Up: offsetY--; break;
            case Direction.Right: offsetX++; break;
            case Direction.Down: offsetY++; break;
            case Direction.Left: offsetX--; break;
        }
    }

    public static float ToAngle(this Direction dir)
    {
        float result = -1;

        if (dir != Direction.NULL)
            result = (int)dir * 90;

        return result;
    }
}

[System.Serializable]
public enum TileType
{
    NULL,
    Player,
    Node,
    Goal,
    Up,
    Right,
    Down,
    Left 
}

public static class TileTypeUtils
{
    public static Direction ToDirection(this TileType tileType)
    {
        Direction result = Direction.NULL;
        switch (tileType)
        {
            case TileType.NULL: result = Direction.NULL; break;
            case TileType.Player: result = Direction.All; break;
            case TileType.Node: result = Direction.All; break;
            case TileType.Goal: result = Direction.NULL; break;
            case TileType.Up: result = Direction.Up; break;
            case TileType.Right: result = Direction.Right; break;
            case TileType.Down: result = Direction.Down; break;
            case TileType.Left: result = Direction.Left; break;
        }
        return result;
    }
}

public enum TileVisibility
{
    Invisible,
    ReadyToVisible,
    Visible
}