using System;
using System.IO;
using System.Runtime.CompilerServices;

using BaseMod.Core.Logger;

namespace YC.GameplayMod;
public class MelonPluginLogger(MelonLoader.MelonLogger.Instance logger) : IPluginLogger
{
    private readonly MelonLoader.MelonLogger.Instance _logger = logger;

    public void Debug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "")
    {
#if DEBUG
        string className = Path.GetFileNameWithoutExtension(filePath);
        _logger.Msg($"[{className}.{callerName} | DEBUG] {message}");
#endif
    }

    public void Info(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        _logger.Msg($"[{className}.{callerName} | INFO] {message}");
    }

    public void Message(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        _logger.Msg($"[{className}.{callerName}] {message}");
    }

    public void Warn(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        _logger.Warning($"[{className}.{callerName}] {message}");
    }

    public void Error(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        _logger.Error($"[{className}.{callerName}] {message}");
    }

    public void Error(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") => Error(exception.Message, callerName, filePath);

    public void Fatal(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        _logger.BigError($"[{className}.{callerName}] {message}");
    }

    public void Fatal(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "") => Fatal(exception.Message, callerName, filePath);
}
