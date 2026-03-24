using System;
using System.Reflection;
using Exiled.API.Features;
using SpectatorDisabler.HarmonyPatches;
using SpectatorDisabler.Exiled.Tower;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Item = Exiled.Events.Handlers.Item;
using Workstation = SpectatorDisabler.Exiled.Tower.Workstation;

namespace SpectatorDisabler.Exiled;

public class SpectatorDisabler : Plugin<Config>
{
    private static HarmonyWrapper? Harmony { get; set; }

    private Workstation Workstation { get; } = new();

    private WindowBlocker WindowBlocker { get; } = new();

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
            Server.WaitingForPlayers += Workstation.OnWaitingForPlayers;
            Item.ChangingAttachments += Workstation.OnAttachmentChange;
            Player.DroppingItem += Workstation.OnDroppingItem;
            Player.PickingUpItem += Workstation.OnPickingUpItem;
        }

        if (Config.TowerWindowBlockers)
        {
            Server.RoundStarted += WindowBlocker.OnRoundStarted;
        }
    }

    private void UnregisterEvents()
    {
        Player.Spawned -= EventHandler.OnPlayerSpawning;

        if (Config.TowerWorkbench)
        {
            Server.WaitingForPlayers -= Workstation.OnWaitingForPlayers;
            Item.ChangingAttachments -= Workstation.OnAttachmentChange;
            Player.DroppingItem -= Workstation.OnDroppingItem;
            Player.PickingUpItem -= Workstation.OnPickingUpItem;
        }

        if (Config.TowerWindowBlockers)
        {
            Server.RoundStarted -= WindowBlocker.OnRoundStarted;
        }
    }
}
