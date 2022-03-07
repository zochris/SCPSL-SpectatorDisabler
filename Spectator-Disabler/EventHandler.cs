using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Respawning;

namespace SpectatorDisabler
{
    public class EventHandler
    {
        private const string RemainingTargetMessage = "Remaining targets: <color=red>$count</color>";

        private readonly SpectatorDisabler _plugin;

        private int _remainingTargetCount;

        public EventHandler(SpectatorDisabler plugin)
        {
            _plugin = plugin;
        }

        public void OnPlayerDied(DiedEventArgs ev)
        {
            var player = ev.Target;
            Timing.CallDelayed(1, () =>
            {
                if (_plugin.Config.ShowRemainingTargetsMessage)
                {
                    _remainingTargetCount = Player.List.Count(p =>
                        p.Role.Team == Team.CDP || p.Role.Team == Team.MTF || p.Role.Team == Team.RSC);

                    var scpPlayers = Player.List.Where(p => p.Role.Side == Side.Scp);
                    BroadcastMessage(scpPlayers,
                        RemainingTargetMessage.Replace("$count", _remainingTargetCount.ToString()));
                }

                if (player.Role != RoleType.Spectator)
                    return;

                player.SetRole(RoleType.Tutorial);
            });
        }

        public void OnPlayerJoined(JoinedEventArgs ev)
        {
            Timing.CallDelayed(2, () => { ev.Player.SetRole(RoleType.Tutorial); });
        }

        public void OnServerRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!_plugin.Config.ShowRemainingTargetsMessage)
                return;

            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
                return;

            _remainingTargetCount += ev.Players.Count;

            var scpPlayers = Player.List.Where(p => p.Role.Side == Side.Scp);
            BroadcastMessage(scpPlayers, RemainingTargetMessage.Replace("$count", _remainingTargetCount.ToString()));
        }


        private void BroadcastMessage(IEnumerable<Player> targets, string message)
        {
            foreach (var player in targets)
                player.Broadcast(_plugin.Config.RemainingTargetsMessageDuration, message);
        }
    }
}
