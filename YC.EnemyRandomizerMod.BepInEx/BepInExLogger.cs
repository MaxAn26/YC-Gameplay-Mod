using System;
using System.IO;
using System.Runtime.CompilerServices;

using BaseMod.Core.Logger;

using BepInEx.Logging;

namespace YC.EnemyRandomizerMod.BepInEx;
public class BepInExLogger(ManualLogSource logger) : IPluginLogger {
    readonly ManualLogSource _logger = logger;

    public void Debug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        var className = Path.GetFileNameWithoutExtension(filePath);
        _logger.LogDebug($"[{className}.{callerName}] {message}");
    }

    public void Info(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        var className = Path.GetFileNameWithoutExtension(filePath);
        _logger.LogInfo($"[{className}.{callerName}] {message}");
    }

    public void Message(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        var className = Path.GetFileNameWithoutExtension(filePath);
        _logger.LogMessage($"[{className}.{callerName}] {message}");
    }

    public void Warn(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        var className = Path.GetFileNameWithoutExtension(filePath);
        _logger.LogWarning($"[{className}.{callerName}] {message}");
    }

    public void Error(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        var className = Path.GetFileNameWithoutExtension(filePath);
        _logger.LogError($"[{className}.{callerName}] {message}");
    }

    public void Error(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        Error(exception.Message, callerName, filePath);
    }

    public void Fatal(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        var className = Path.GetFileNameWithoutExtension(filePath);
        _logger.LogFatal($"[{className}.{callerName}] {message}");
    }

    public void Fatal(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") {
        Fatal(exception.Message, callerName, filePath);
    }
}