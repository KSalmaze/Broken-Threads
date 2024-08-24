using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Debuger : MonoBehaviour
{
    private string _logFilePath;

    void Start()
    {
        _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "GameLog.txt");
        Debug.Log("Custom Logger Initialized. Log file at: " + _logFilePath);
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    private void LogMessage(string logString, string stackTrace, LogType type)
    {
        File.AppendAllText(_logFilePath, $"{type}: {logString}\n{stackTrace}\n");
    }
}
