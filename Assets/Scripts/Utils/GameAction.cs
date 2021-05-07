using UnityEngine;
using Level;

public class GameAction : MonoBehaviour {
    public void ResetGame() {
        LevelManager.Main.LoadLevel("1/1/1");
    }
}