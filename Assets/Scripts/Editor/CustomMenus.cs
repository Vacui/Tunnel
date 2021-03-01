using UnityEditor;
using UnityEngine;

public static class CustomMenus {
   
    [MenuItem("Custom/Take Screenshot")]
    private static void GameScreenshot() {
        string screenshotPath = Application.persistentDataPath + "/Screenshot.png";
        ScreenCapture.CaptureScreenshot(screenshotPath);
        Debug.Log($"Saved screenshot " + screenshotPath);
    }

}