using Exiled.API.Features;
using Exiled.API.Features.Toys;
using SpectatorDisabler.TowerTools;
using UnityEngine;

namespace SpectatorDisabler.Tower;

internal class WindowBlocker : WindowBlockerBase
{
    override protected void CreatePlane(Vector3 position, Vector3 eulerRotation, Vector3 scale)
    {
        Primitive.Create(PrimitiveType.Plane, position, eulerRotation, scale);
    }

    public void OnRoundStarted()
    {
        SpawnWindowBlockers();
    }

    override protected void LogDebug(string message)
    {
        Log.Debug(message);
    }
}
