using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using PlayerRoles;

namespace SpectatorDisabler.LabApi;

public class SpectatorDisablerHandler : CustomEventsHandler
{
    public override void OnPlayerChangingRole(PlayerChangingRoleEventArgs ev)
    {
        base.OnPlayerChangingRole(ev);

        if (ev.NewRole == RoleTypeId.Spectator)
        {
            ev.NewRole = RoleTypeId.Tutorial;
        }
    }
}
