using System;
using System.Reflection;
using Exiled.API.Features;
using SpectatorDisabler.HarmonyPatches;
using SpectatorDisabler.Tower;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Item = Exiled.Events.Handlers.Item;

namespace SpectatorDisabler;

public class SpectatorDisabler : Plugin<Config>
{
    private static HarmonyWrapper? Harmony { get; set; }

    public override string Author => "zochris";

    public override string Name => "SpectatorDisabler";

    public override Version RequiredExiledVersion { get; } = new(9, 12, 6);

    public override Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

    public override void OnDisabled()
    {
        Harmony?.Disable();

        UnregisterEvents();

        Log.Info("SpectatorDisabler unloaded");
    }

    public override void OnEnabled()
    {
        Harmony = new HarmonyWrapper(Name, new HarmonyHelper());
        Harmony.Enable();

        RegisterEvents();

        Log.Info($"SpectatorDisabler {Version} loaded");
    }

    private void RegisterEvents()
    {
        Log.Debug("Setting up event handler");

        Player.Spawned += EventHandler.OnPlayerSpawning;

        if (Config.TowerWorkbench)
        {
            Server.WaitingForPlayers += TowerBench.OnWaitingForPlayers;
            Item.ChangingAttachments += TowerBench.OnAttachmentChange;
            Player.DroppingItem += TowerBench.OnDroppingItem;
            Player.PickingUpItem += TowerBench.OnPickingUpItem;
        }

        if (Config.TowerWindowBlockers)
        {
            Server.RoundStarted += TowerWindowBlockers.OnRoundStarted;
        }
    }

    private void UnregisterEvents()
    {
        Player.Spawned -= EventHandler.OnPlayerSpawning;

        if (Config.TowerWorkbench)
        {
            Server.WaitingForPlayers -= TowerBench.OnWaitingForPlayers;
            Item.ChangingAttachments -= TowerBench.OnAttachmentChange;
            Player.DroppingItem -= TowerBench.OnDroppingItem;
            Player.PickingUpItem -= TowerBench.OnPickingUpItem;
        }

        if (Config.TowerWindowBlockers)
        {
            Server.RoundStarted -= TowerWindowBlockers.OnRoundStarted;
        }
    }
}
