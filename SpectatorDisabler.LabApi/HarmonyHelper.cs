using System.Linq;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SpectatorDisabler.HarmonyPatches;

namespace SpectatorDisabler.LabApi;

public class HarmonyHelper : IHarmonyHelper
{
    public void SendMessage(string message, ushort duration, RoleTypeId targetRole)
    {
        var targets = Player.List.Where(player => player.Role == targetRole);

        foreach (var target in targets)
        {
            target.SendBroadcast(message, duration);
        }
    }

    public void LogDebug(string message)
    {
        Logger.Debug(message);
    }

    public void LogError(string message)
    {
        Logger.Error(message);
    }

    public void LogWarn(string message)
    {
        Logger.Warn(message);
    }

    public void LogInfo(string message)
    {
        Logger.Info(message);
    }
}
