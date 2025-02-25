using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SpectatorDisabler.HarmonyPatches;

namespace SpectatorDisabler;

public class HarmonyHelper : IHarmonyHelper
{
    public void SendMessage(string message, ushort duration, RoleTypeId targetRole)
    {
        var targets = Player.List.Where(player => player.Role == targetRole);

        foreach (var target in targets)
        {
            target.Broadcast(duration, message);
        }
    }

    public void LogDebug(string message)
    {
        Log.Info(message);
    }

    public void LogError(string message)
    {
        Log.Error(message);
    }

    public void LogWarn(string message)
    {
        Log.Warn(message);
    }

    public void LogInfo(string message)
    {
        Log.Info(message);
    }
}
