using System.Runtime.CompilerServices;

namespace BaseMod.Core.Logger;
public interface IPluginLogger {
    void Debug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Error(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Error(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Fatal(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Fatal(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Info(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Message(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
    void Warn(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "");
}