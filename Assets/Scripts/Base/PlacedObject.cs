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

    [SerializeField] List<Direction> openDirectionsList;

    public bool IsDirectionOpen(List<Direction> openDirectionsList, Direction dir) {
        if (openDirectionsList != null) {
            if (openDirectionsList.Count > 0)
                return openDirectionsList.Contains(dir);
        } else {
            openDirectionsList = new List<Direction>();
        }
        return false;
    }
    public bool IsDirectionOpen(Direction dir) {
        return IsDirectionOpen(openDirectionsList, dir);
    }
    internal Direction GetOtherDirection(Direction dir) {
        return GetOtherDirection(openDirectionsList, dir);
    }

    public void Discover() {
        
    }

}