using EXILED;
using EXILED.Extensions;
using MEC;

namespace SpectatorDisabler
{
    public class EventHandler
    {
        public void OnPlayerDeathEvent(ref PlayerDeathEvent ev)
        {
            var player = ev.Player;
            Timing.CallDelayed(2, () =>
            {
                player.SetRole(RoleType.Tutorial);

                // add player to list of dead players, so that they get respawned
                EventPlugin.DeadPlayers.Add(player);
            });
        }
    }
}
