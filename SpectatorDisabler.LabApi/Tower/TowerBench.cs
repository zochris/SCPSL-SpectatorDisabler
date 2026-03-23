using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SpectatorDisabler.LabApi.Tower;

public class TowerBench : CustomEventsHandler
{
    private const float WeaponSpawnZMargin = -1f;

    private const float WeaponSpawnYMargin = -0.35f;

    private readonly static Vector3 BenchSpawnPosition = new(42.93f, 313.13f, -34.55f);

    private readonly static Vector3 BenchSpawnRotation = new(0f, -90f, 0f);

    private readonly static Vector3 InitialSpawn = new(42.9f, 315.25f, -31f);

    private readonly static List<Pickup> WallItems = [];

    private readonly static List<uint> GivenWallItems = [];

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

    private static void SpawnWorkbench()
    {
        Logger.Debug("Instantiating bench.");

        // TODO: cleanup, maybe extract method
        var workstationPrefab = NetworkClient.prefabs.FirstOrDefault(x => x.Value.name.Contains("Work Station")).Value;
        var workstation = Object.Instantiate(workstationPrefab, BenchSpawnPosition, Quaternion.Euler(BenchSpawnRotation));
        NetworkServer.Spawn(workstation);

        // TODO: this is duplicating the EXILED part a lot, maybe put into extra project like Harmony patches and only implement specific details here? Or drop EXILED support entirely and just do everything here?

        // TODO: since there are multiple projects consider building a single binary?
    }

    private static void SpawnWallWeapons()
    {
        Logger.Debug("Spawning tower wall weapons.");

        var yOffset = 0;
        var zOffset = 0;

        foreach (var spawn in WeaponsToSpawn)
        {
            var newPosition = new Vector3(
                InitialSpawn.x + spawn.Offset.x,
                InitialSpawn.y + yOffset * WeaponSpawnYMargin + spawn.Offset.y,
                InitialSpawn.z + zOffset * WeaponSpawnZMargin + spawn.Offset.z);

            var type = spawn.Type;
            var pickup = Pickup.Create(type, newPosition, Quaternion.Euler(spawn.Rotation));

            if (pickup is null)
            {
                Logger.Error("Failed to create pickup");
                continue;
            }

            pickup.Rigidbody?.isKinematic = true;
            WallItems.Add(pickup);

            pickup.Spawn();

            yOffset++;

            if (yOffset >= WeaponsToSpawn.Length / 2)
            {
                zOffset++;
                yOffset = 0;
            }
        }
    }

    public override void OnServerWaitingForPlayers()
    {
        base.OnServerWaitingForPlayers();

        WallItems.Clear();
        GivenWallItems.Clear();
        SpawnWallWeapons();
        SpawnWorkbench();
    }

    public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
    {
        base.OnPlayerDroppingItem(ev);

        if (!GivenWallItems.Contains(ev.Item.Serial))
        {
            return;
        }

        ev.Player.RemoveItem(ev.Item);
        ev.IsAllowed = false;
        GivenWallItems.Remove(ev.Item.Serial);
    }

    public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
    {
        base.OnPlayerPickingUpItem(ev);

        if (!WallItems.Contains(ev.Pickup))
        {
            return;
        }

        var otherPickup = Pickup.Create(ev.Pickup.Type, Vector3.zero);

        if (otherPickup is null)
        {
            Logger.Error("Failed to create pickup");
            return;
        }

        var itemInInventory = ev.Player.AddItem(otherPickup);

        if (itemInInventory is null)
        {
            Logger.Error("Pickup could not be added to inventory");
            return;
        }

        GivenWallItems.Add(itemInInventory.Serial);

        if (itemInInventory.Base is Firearm weaponInInventory)
        {
            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(ev.Player.ReferenceHub, out var preferences))
            {
                preferences.TryGetValue(itemInInventory.Type, out var items);
                weaponInInventory.ApplyAttachmentsCode(items, true);
            }
        }

        ev.IsAllowed = false;
    }

    // Changes preference immediately if the player is a tutorial.
    public override void OnPlayerChangedAttachments(PlayerChangedAttachmentsEventArgs ev)
    {
        base.OnPlayerChangedAttachments(ev);

        if (ev.Player.Role != RoleTypeId.Tutorial)
        {
            return;
        }

        if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(ev.Player.ReferenceHub, out var preferences))
        {
            preferences[ev.FirearmItem.Type] = ev.NewAttachments;
        }
    }

    private sealed class WallWeaponSpawn(ItemType weapon, Vector3 rotation, Vector3 offset)
    {
        public readonly Vector3 Offset = offset;

        public readonly Vector3 Rotation = rotation;

        public readonly ItemType Type = weapon;
    }
}
