using System;
using Exiled.API.Features;
using HarmonyLib;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace SpectatorDisabler
{
    public class SpectatorDisabler : Plugin<Config>
    {
        private static int _harmonyCounter;
        private EventHandler _eventHandler;

        private static Harmony HarmonyInstance { get; set; }

        public override string Name { get; } = "SpectatorDisabler";
        public override string Author { get; } = "zochris";
        public override Version RequiredExiledVersion { get; } = new Version(2, 0, 10);

        public override void OnEnabled()
        {
            HarmonyInstance = new Harmony($"{Name}{_harmonyCounter++}");
            HarmonyInstance.PatchAll();

            RegisterEvents();

            Log.Info("SpectatorDisabler loaded");
        }

        public override void OnDisabled()
        {
            if (HarmonyInstance != null || HarmonyInstance != default)
                HarmonyInstance.UnpatchAll();

            UnregisterEvents();

            _eventHandler = null;
            Log.Info("SpectatorDisabler unloaded");
        }

        private void RegisterEvents()
        {
            Log.Debug("Setting up event handler");

            _eventHandler = new EventHandler(this);
            Player.Died += _eventHandler.OnPlayerDied;
            Player.Joined += _eventHandler.OnPlayerJoined;
            Server.RespawningTeam += _eventHandler.OnServerRespawningTeam;
        }

        private void UnregisterEvents()
        {
            Player.Died -= _eventHandler.OnPlayerDied;
            Player.Joined -= _eventHandler.OnPlayerJoined;
            Server.RespawningTeam -= _eventHandler.OnServerRespawningTeam;
            _eventHandler = null;
        }
    }
}
