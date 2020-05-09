using EXILED;
using Harmony;
using SpectatorDisabler.Patches;

namespace SpectatorDisabler
{
    public class Plugin : EXILED.Plugin
    {
        private static int _harmonyCounter;
        private EventHandler _eventHandler;

        private static HarmonyInstance HarmonyInstance { get; set; }

        public override string getName { get; } = "io.github.zochris.SpectatorDisabler";

        public override void OnEnable()
        {
            // disable RespawnPatch of EXILED framework
            EventPlugin.RespawnPatchDisable = true;

            HarmonyInstance = HarmonyInstance.Create($"{getName}{_harmonyCounter++}");

            // setup patch for compiler generated _Update() method of MTFRespawn with transpiler
            Log.Debug("Setting up _Update() patch");
            var originalUpdateType = AccessTools.Inner(typeof(MTFRespawn), "<_Update>d__21");
            var originalUpdateMoveNextMethod = AccessTools.Method(originalUpdateType, "MoveNext");
            var transpiler = typeof(MTFRespawn_UpdatePatch).GetMethod(nameof(MTFRespawn_UpdatePatch.Transpiler));

            HarmonyInstance.Patch(originalUpdateMoveNextMethod, transpiler: new HarmonyMethod(transpiler));

            // setup patch for RespawnDeadPlayers() of MTFRespawn
            Log.Debug("Setting up RespawnDeadPlayers() patch");
            var originalRespawn = AccessTools.Method(typeof(MTFRespawn), nameof(MTFRespawn.RespawnDeadPlayers));
            var respawnPrefix = AccessTools.Method(typeof(MTFRespawnRespawnDeadPlayersPatch),
                nameof(MTFRespawnRespawnDeadPlayersPatch.Prefix));

            HarmonyInstance.Patch(originalRespawn, new HarmonyMethod(respawnPrefix));

            // setup patch for CallCmdRecallPlayer() of Scp049PlayerScript
            Log.Debug("Setting up CallCmdRecallPlayer() patch");
            var originalRecallPlayer =
                AccessTools.Method(typeof(Scp049PlayerScript), nameof(Scp049PlayerScript.CallCmdRecallPlayer));
            var transpilerRecallPlayer = AccessTools.Method(typeof(Scp049PlayerScriptCallCmdRecallPlayerPatch),
                nameof(Scp049PlayerScriptCallCmdRecallPlayerPatch.Transpiler));

            HarmonyInstance.Patch(originalRecallPlayer, transpiler: new HarmonyMethod(transpilerRecallPlayer));

            Log.Debug("Setting up event handler");
            _eventHandler = new EventHandler();
            Events.PlayerDeathEvent += _eventHandler.OnPlayerDeathEvent;
            Events.PlayerJoinEvent += _eventHandler.OnPlayerJoinEvent;
            Events.TeamRespawnEvent += _eventHandler.OnTeamRespawnEvent;

            Log.Info("SpectatorDisabler loaded");
        }

        public override void OnDisable()
        {
            EventPlugin.RespawnPatchDisable = false;

            if (HarmonyInstance != null || HarmonyInstance != default)
                HarmonyInstance.UnpatchAll();

            Events.PlayerDeathEvent -= _eventHandler.OnPlayerDeathEvent;
            Events.PlayerJoinEvent -= _eventHandler.OnPlayerJoinEvent;
            Events.TeamRespawnEvent -= _eventHandler.OnTeamRespawnEvent;

            _eventHandler = null;
            Log.Info("SpectatorDisabler unloaded");
        }

        public override void OnReload()
        {
        }
    }
}
