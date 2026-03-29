using System.Linq;
using LabApi.Features.Wrappers;
using Mirror;
using SpectatorDisabler.TowerTools;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SpectatorDisabler.LabApi.Tower;

internal class Workstation : WorkstationBase
{
    override protected void LogDebug(string message)
    {
        Logger.Debug(message);
    }

    override protected void LogError(string message)
    {
        Logger.Error(message);
    }

    override protected uint SpawnWallPickup(ItemType type, Vector3 position, Quaternion rotation)
    {
        var pickup = Pickup.Create(type, position, rotation);

        if (pickup is null)
        {
            Logger.Error("Failed to create pickup");
            return 0;
        }

        pickup.Rigidbody?.isKinematic = true;
        pickup.Spawn();

        return pickup.Serial;
    }

    override protected void SpawnWorkbenchObject(Vector3 position, Quaternion rotation)
    {
        var workstationPrefab = NetworkClient.prefabs
            .FirstOrDefault(x => x.Value.name.Contains("Work Station")).Value;
        var workstation = Object.Instantiate(workstationPrefab, position, rotation);
        NetworkServer.Spawn(workstation);
    }

    // Expose Initialize so the outer class can call it.
    public new void Initialize()
    {
        base.Initialize();
    }
}
