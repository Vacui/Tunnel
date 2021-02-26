using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Direction {
    NULL,
    Up,
    Right,
    Down,
    Left
}

public class PlacedObject : MonoBehaviour {

    public bool isSafe;
    public List<Direction> openDirectionsList;

    public bool IsDirectionOpen(List<Direction> openDirectionsList, Direction dir) {
        return openDirectionsList.Count > 0 ? openDirectionsList.Contains(dir) : false;
    }
    public bool IsDirectionOpen(Direction dir) {
        return IsDirectionOpen(openDirectionsList, dir);
    }
    internal Direction GetOtherDirection(Direction dir) {
        return GetOtherDirection(openDirectionsList, dir);
    }

    public Direction GetOtherDirection(List<Direction> openDirectionsList, Direction dir) {
        List<Direction> tempOpenDirectionsList = new List<Direction>();
        tempOpenDirectionsList.AddRange(openDirectionsList);
        if (tempOpenDirectionsList.Contains(dir))
            tempOpenDirectionsList.Remove(dir);
        if (tempOpenDirectionsList.Count > 0)
            return tempOpenDirectionsList[0];
        else
            return Direction.NULL;
    }

    public bool IsSafe() {
        return isSafe;
    }

    public void Discover() {
        
    }

    private void Reset() {
        if (openDirectionsList == null) openDirectionsList = new List<Direction>();
    }

}