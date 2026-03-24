using LabApi.Features.Wrappers;
using SpectatorDisabler.TowerTools;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SpectatorDisabler.LabApi.Tower;

internal sealed class WindowBlocker : WindowBlockerBase
{
    override protected void LogDebug(string message)
    {
        Logger.Debug(message);
    }

    override protected void CreatePlane(Vector3 position, Vector3 eulerRotation, Vector3 scale)
    {
        var plane = PrimitiveObjectToy.Create(position, Quaternion.Euler(eulerRotation), scale);
        plane.Type = PrimitiveType.Plane;
    }
}
