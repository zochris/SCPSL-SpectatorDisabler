using System;
using System.Reflection;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using SpectatorDisabler.HarmonyPatches;
using SpectatorDisabler.LabApi.Tower;

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

    private TowerBench TowerBench { get; } = new();

    private TowerWindowBlockers TowerWindowBlockers { get; } = new();

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

        if (Config?.TowerWorkbench ?? false)
        {
            CustomHandlersManager.RegisterEventsHandler(TowerBench);
        }

        if (Config?.TowerWindowBlockers ?? false)
        {
            CustomHandlersManager.RegisterEventsHandler(TowerWindowBlockers);
        }
    }

    private void UnregisterEvents()
    {
        CustomHandlersManager.UnregisterEventsHandler(SpectatorDisablerHandler);

        if (Config?.TowerWorkbench ?? false)
        {
            CustomHandlersManager.UnregisterEventsHandler(TowerBench);
        }

        if (Config?.TowerWindowBlockers ?? false)
        {
            CustomHandlersManager.UnregisterEventsHandler(TowerWindowBlockers);
        }
    }
}
