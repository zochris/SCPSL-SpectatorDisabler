using LabApi.Events.CustomHandlers;

namespace SpectatorDisabler.LabApi.Tower;

public class WindowBlockerEventHandler : CustomEventsHandler
{
    private readonly WindowBlocker _windowBlocker = new();

    public override void OnServerRoundStarted()
    {
        base.OnServerRoundStarted();
        _windowBlocker.SpawnWindowBlockers();
    }
}
