using UnityEngine;

public class DebugLogToScreen : MonoBehaviour
{
    private string logText = "";

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Log(string logString, string stackTrace, LogType type)
    {
        logText += logString + "\n";
        if (logText.Length > 2000)
            logText = logText.Substring(logText.Length - 2000);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 800, 600), logText);
    }
}
