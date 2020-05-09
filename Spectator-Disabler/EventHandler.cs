using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;
using MEC;

namespace SpectatorDisabler
{
    public class EventHandler
    {
        private const string RemainingTargetMessage = "Remaining targets: <color=red>$count</color>";

        private int _remainingTargetCount;

        public void OnPlayerDeathEvent(ref PlayerDeathEvent ev)
        {
            var player = ev.Player;
            Timing.CallDelayed(1, () =>
            {
                _remainingTargetCount = Player.GetHubs().Count(p =>
                    p.GetTeam() == Team.CDP && p.GetTeam() == Team.MTF && p.GetTeam() == Team.RSC);

                var scpPlayers = Team.SCP.GetHubs();
                BroadcastMessage(scpPlayers,
                    RemainingTargetMessage.Replace("$count", _remainingTargetCount.ToString()));

                if (player.GetRole() != RoleType.Spectator)
                    return;

                player.SetRole(RoleType.Tutorial);

                // add player to list of dead players, so that they get respawned
                EventPlugin.DeadPlayers.Add(player);
            });
        }

        public void OnPlayerJoinEvent(PlayerJoinEvent ev)
        {
            Timing.CallDelayed(2, () =>
            {
                ev.Player.SetRole(RoleType.Tutorial);
                EventPlugin.DeadPlayers.Add(ev.Player);
            });
        }

        public void OnTeamRespawnEvent(ref TeamRespawnEvent ev)
        {
            if (ev.IsChaos)
                return;

            _remainingTargetCount += ev.ToRespawn.Count;

            var scpPlayers = Team.SCP.GetHubs();
            BroadcastMessage(scpPlayers, RemainingTargetMessage.Replace("$count", _remainingTargetCount.ToString()));
        }


        private static void BroadcastMessage(IEnumerable<ReferenceHub> targets, string message)
        {
            foreach (var player in targets)
                player.GetComponent<Broadcast>()
                    .TargetAddElement(player.scp079PlayerScript.connectionToClient, message, 5, false);
        }
    }
}
