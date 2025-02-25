using System.Linq;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;

namespace SpectatorDisabler.LabApi;

public class SpectatorDisablerHandler : CustomEventsHandler
{
    public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
    {
        base.OnPlayerChangedRole(ev);

        if (ev.Player.Role == RoleTypeId.Spectator)
        {
            Timing.CallDelayed(1, () => { ev.Player.SetRole(RoleTypeId.Tutorial, RoleChangeReason.Died, RoleSpawnFlags.UseSpawnpoint); });
        }

        // TODO: is this still required?
        if (ev.ChangeReason == RoleChangeReason.Revived)
        {
            var scp = Player.List.FirstOrDefault(player => player.Role == RoleTypeId.Scp049);

            if (scp != null)
            {
                ev.Player.Position = scp.Position;
            }
        }
    }
}
