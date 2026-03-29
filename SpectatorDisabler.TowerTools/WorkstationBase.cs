using System.Collections.Generic;
using UnityEngine;

namespace SpectatorDisabler.TowerTools;

public abstract class WorkstationBase
{
    private const float WeaponSpawnZMargin = -1f;

    private const float WeaponSpawnYMargin = -0.35f;

    private readonly static Vector3 BenchSpawnPosition = new(42.93f, 313.13f, -34.55f);

    private readonly static Vector3 BenchSpawnRotation = new(0f, -90f, 0f);

    private readonly static Vector3 InitialSpawn = new(42.9f, 315.25f, -31f);

    // These were all figured out by trial and error, someone with a model editor
    // and an affinity for figuring out the math to make these spawn perfectly in order
    // is free to do so
    private readonly static WallWeaponSpawn[] WeaponsToSpawn =
    [
        new(ItemType.GunCOM15, new Vector3(0, 0, 0), new Vector3(0, -0.1f, 0)),
        new(ItemType.GunCOM18, new Vector3(0, 0, 0), new Vector3(-0.03f, 0, -0.1f)),
        new(ItemType.GunRevolver, new Vector3(0, 90, 0), new Vector3(0, 0, 0)),
        new(ItemType.GunFSP9, new Vector3(0, 180, 0), new Vector3(0, -0.02f, 0.1f)),
        new(ItemType.GunCrossvec, new Vector3(0, 180, 0), new Vector3(0, 0, 0)),
        new(ItemType.GunE11SR, new Vector3(0, 180, 0), new Vector3(0, -0.05f, 0.2f)),
        new(ItemType.GunAK, new Vector3(0, 0, 0), new Vector3(0, 0.07f, 0)),
        new(ItemType.GunShotgun, new Vector3(0, 0, 0), new Vector3(0, -0.1f, 0)),
        new(ItemType.GunFRMG0, new Vector3(0, 180, 0), new Vector3(0, -0.1f, 0)),
        new(ItemType.GunLogicer, new Vector3(0, -90, 0), new Vector3(0, 0, 0.1f)),
        new(ItemType.GunA7, new Vector3(0, 0, 0), new Vector3(0, 0.07f, 0)),
        new(ItemType.GunCom45, new Vector3(0, 0, 0), new Vector3(0, 0, 0))
    ];

    private readonly List<uint> _givenWallItems = [];

    private readonly HashSet<uint> _wallItemSerials = [];

    // Returns the serial of the spawned pickup, or 0 on failure.
    protected abstract uint SpawnWallPickup(ItemType type, Vector3 position, Quaternion rotation);

    protected abstract void SpawnWorkbenchObject(Vector3 position, Quaternion rotation);

    protected abstract void LogDebug(string message);

    protected abstract void LogError(string message);

    private void SpawnWorkbench()
    {
        LogDebug("Instantiating bench.");
        SpawnWorkbenchObject(BenchSpawnPosition, Quaternion.Euler(BenchSpawnRotation));
    }

    private void SpawnWallWeapons()
    {
        LogDebug("Spawning tower wall weapons.");

        var yOffset = 0;
        var zOffset = 0;

        foreach (var spawn in WeaponsToSpawn)
        {
            var position = new Vector3(
                InitialSpawn.x + spawn.Offset.x,
                InitialSpawn.y + yOffset * WeaponSpawnYMargin + spawn.Offset.y,
                InitialSpawn.z + zOffset * WeaponSpawnZMargin + spawn.Offset.z);

            var serial = SpawnWallPickup(spawn.Type, position, Quaternion.Euler(spawn.Rotation));

            if (serial != 0)
            {
                _wallItemSerials.Add(serial);
            }

            yOffset++;

            if (yOffset >= WeaponsToSpawn.Length / 2)
            {
                zOffset++;
                yOffset = 0;
            }
        }
    }

    /// <summary>
    ///     Clears tracked state and spawns all wall weapons and the workbench.
    ///     Call this from the WaitingForPlayers / equivalent event.
    /// </summary>
    protected void Initialize()
    {
        _wallItemSerials.Clear();
        _givenWallItems.Clear();
        SpawnWallWeapons();
        SpawnWorkbench();
    }

    public bool IsWallItem(uint serial)
    {
        return _wallItemSerials.Contains(serial);
    }

    public bool IsGivenItem(uint serial)
    {
        return _givenWallItems.Contains(serial);
    }

    public void TrackGivenItem(uint serial)
    {
        _givenWallItems.Add(serial);
    }

    public void RemoveGivenItem(uint serial)
    {
        _givenWallItems.Remove(serial);
    }
}
