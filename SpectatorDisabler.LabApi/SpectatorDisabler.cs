using System;
using System.Reflection;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using SpectatorDisabler.HarmonyPatches;

namespace SpectatorDisabler.LabApi;

public class SpectatorDisabler : Plugin<Config>
{
    public override string Description => "Disables the spectator role to prevent meta gaming.";

    public override string Author => "zochris";

    public override string Name => "SpectatorDisabler";

    public override Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    private HarmonyWrapper? Harmony { get; set; }

    private SpectatorDisablerHandler SpectatorDisablerHandler { get; } = new();

    public override void Enable()
    {
        Harmony = new HarmonyWrapper(Name, new HarmonyHelper());
        Harmony.Enable();

        RegisterEvents();

        Logger.Info($"SpectatorDisabler {Version} loaded");
    }

    public override void Disable()
    {
        Harmony?.Disable();

        UnregisterEvents();

        Logger.Info("SpectatorDisabler unloaded");
    }

    private void RegisterEvents()
    {
        CustomHandlersManager.RegisterEventsHandler(SpectatorDisablerHandler);

        // Logger.Debug("Setting up event handler");
        //
        // Player.Spawned += EventHandler.OnPlayerSpawning;
        // Scp049.FinishingRecall += EventHandler.OnFinishingRecall;
        //
        // if (Config.TowerWorkbench)
        // {
        //     Server.RoundStarted += TowerBench.OnRoundStarted;
        //     Item.ChangingAttachments += TowerBench.OnAttachmentChange;
        //     Player.DroppingItem += TowerBench.OnDroppingItem;
        //     Player.PickingUpItem += TowerBench.OnPickingUpItem;
        // }
        //
        // if (Config.TowerWindowBlockers)
        // {
        //     Server.RoundStarted += TowerWindowBlockers.OnRoundStarted;
        // }
    }

    private void UnregisterEvents()
    {
        CustomHandlersManager.UnregisterEventsHandler(SpectatorDisablerHandler);

        // Player.Spawned -= EventHandler.OnPlayerSpawning;
        // Scp049.FinishingRecall -= EventHandler.OnFinishingRecall;
        //
        // if (Config.TowerWorkbench)
        // {
        //     Server.RoundStarted -= TowerBench.OnRoundStarted;
        //     Item.ChangingAttachments -= TowerBench.OnAttachmentChange;
        //     Player.DroppingItem -= TowerBench.OnDroppingItem;
        //     Player.PickingUpItem -= TowerBench.OnPickingUpItem;
        // }
        //
        // if (Config.TowerWindowBlockers)
        // {
        //     Server.RoundStarted -= TowerWindowBlockers.OnRoundStarted;
        // }
    }
}
