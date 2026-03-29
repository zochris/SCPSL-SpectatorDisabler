using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace SpectatorDisabler.Exiled;

public static class EventHandler
{
    public static void OnPlayerChangingRole(ChangingRoleEventArgs ev)
    {
        if (ev.NewRole == RoleTypeId.Spectator)
        {
            ev.NewRole = RoleTypeId.Tutorial;
        }
    }
}
