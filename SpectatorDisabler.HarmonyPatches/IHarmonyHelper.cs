using PlayerRoles;

namespace SpectatorDisabler.HarmonyPatches;

public interface IHarmonyHelper
{
    /// <summary>
    /// Display a message to players of a specific role for the given duration.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="duration">The duration the message should be shown.</param>
    /// <param name="targetRole">The role that the message is shown to.</param>
    void SendMessage(string message, ushort duration, RoleTypeId targetRole);

    /// <summary>
    /// Log a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogDebug(string message);

    /// <summary>
    /// Log an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogError(string message);

    /// <summary>
    /// Log a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogWarn(string message);

    /// <summary>
    /// Log an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogInfo(string message);
}
