using EXILED;
using Harmony;
using SpectatorDisabler.Patches;

namespace SpectatorDisabler
{
    public class Plugin : EXILED.Plugin
    {
        private EventHandler _eventHandler;
        private static HarmonyInstance HarmonyInstance { get; set; }
        private static int _harmonyCounter;

        public override void OnEnable()
        {
            // disable RespawnPatch of EXILED framework
            EventPlugin.RespawnPatchDisable = true;

            Log.Debug("Setting up MTFRespawn patches");

            HarmonyInstance = HarmonyInstance.Create($"{getName}{_harmonyCounter++}");

            // setup patch for compiler generated _Update() method of MTFRespawn with transpiler
            var originalUpdateType = AccessTools.Inner(typeof(MTFRespawn), "<_Update>d__21");
            var originalUpdateMoveNextMethod = AccessTools.Method(originalUpdateType, "MoveNext");
            var transpiler = typeof(MTFRespawn_UpdatePatch).GetMethod(nameof(MTFRespawn_UpdatePatch.Transpiler));

            HarmonyInstance.Patch(originalUpdateMoveNextMethod, transpiler: new HarmonyMethod(transpiler));

            // setup patch for RespawnDeadPlayers() of MTFRespawn
            var originalRespawn = typeof(MTFRespawn).GetMethod(nameof(MTFRespawn.RespawnDeadPlayers));
            var respawnPrefix =
                typeof(MTFRespawnRespawnDeadPlayersPatch).GetMethod(nameof(MTFRespawnRespawnDeadPlayersPatch.Prefix));

            HarmonyInstance.Patch(originalRespawn, new HarmonyMethod(respawnPrefix));

            Log.Debug("Setting up event handler");
            _eventHandler = new EventHandler();
            Events.PlayerDeathEvent += _eventHandler.OnPlayerDeathEvent;

            Log.Info("SpectatorDisabler loaded");
        }

        public override void OnDisable()
        {
            EventPlugin.RespawnPatchDisable = false;

            if (HarmonyInstance != null || HarmonyInstance != default)
            {
                HarmonyInstance.UnpatchAll();
            }

            Events.PlayerDeathEvent -= _eventHandler.OnPlayerDeathEvent;

            _eventHandler = null;
            Log.Info("SpectatorDisabler unloaded");
        }

        public override void OnReload()
        {
        }

        public override string getName { get; } = "io.github.zochris.SpectatorDisabler";
    }
}
