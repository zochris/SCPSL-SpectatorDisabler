using System;
using System.Linq;
using EXILED;
using EXILED.Extensions;

namespace SpectatorDisabler.Patches
{
    public class MTFRespawnRespawnDeadPlayersPatch
    {
        public static bool Prefix(MTFRespawn __instance)
        {
            // mostly the original code from the EXILED framework patch
            try
            {
                __instance.playersToNTF.Clear();

                Log.Debug($"Respawn: Got players: {EventPlugin.DeadPlayers.Count}");

                foreach (var player in EventPlugin.DeadPlayers.ToArray())
                {
                    // also include tutorial role
                    if (player.GetOverwatch() ||
                        player.GetRole() != RoleType.Spectator && player.GetRole() != RoleType.Tutorial)
                    {
                        Log.Debug($"Removing {player.GetNickname()} -- Overwatch true, not spectator or not tutorial");
                        EventPlugin.DeadPlayers.Remove(player);
                    }
                }

                if (EXILED.Plugin.Config.GetBool("exiled_random_respawns"))
                    EventPlugin.DeadPlayers.ShuffleList();

                var isChaos = __instance.nextWaveIsCI;
                var maxRespawn = isChaos ? __instance.maxCIRespawnAmount : __instance.maxMTFRespawnAmount;

                var playersToRespawn = EventPlugin.DeadPlayers.Take(maxRespawn).ToList();
                Log.Debug($"Respawn: pre-event list: {playersToRespawn.Count}");
                Events.InvokeTeamRespawn(ref isChaos, ref maxRespawn, ref playersToRespawn);

                if (maxRespawn <= 0 || playersToRespawn == null || playersToRespawn.Count == 0)
                    return false;

                var num = 0;
                foreach (var player in playersToRespawn.TakeWhile(player => num < maxRespawn)
                    .Where(player => player != null))
                {
                    ++num;
                    if (isChaos)
                    {
                        __instance.GetComponent<CharacterClassManager>()
                            .SetPlayersClass(RoleType.ChaosInsurgency, player.gameObject);
                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, player.GetNickname() + " (" +
                                                                          player.GetUserId() +
                                                                          ") respawned as Chaos Insurgency agent.",
                            ServerLogs.ServerLogType.GameEvent);
                    }
                    else
                    {
                        __instance.playersToNTF.Add(player.gameObject);
                    }

                    EventPlugin.DeadPlayers.Remove(player);
                }

                if (num <= 0) return false;

                ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
                    (__instance.nextWaveIsCI ? "Chaos Insurgency" : "MTF") + " respawned!",
                    ServerLogs.ServerLogType.GameEvent);

                if (__instance.nextWaveIsCI)
                    __instance.Invoke("CmdDelayCIAnnounc", 1f);

                __instance.SummonNTF();
                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"RespawnEvent error: {exception}");
                return true;
            }
        }
    }
}
