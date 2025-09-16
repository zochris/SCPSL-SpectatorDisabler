using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using FirearmPickup = Exiled.API.Features.Pickups.FirearmPickup;
using Pickup = Exiled.API.Features.Pickups.Pickup;

namespace SpectatorDisabler.Tower;

internal static class TowerBench
{
    private const float WeaponSpawnZMargin = -1f;

    private const float WeaponSpawnYMargin = -0.35f;

    private readonly static Vector3 BenchSpawnPosition = new(42.93f, 313.13f, -34.55f);

    private readonly static Vector3 BenchSpawnRotation = new(0f, -90f, 0f);

    private readonly static Vector3 InitialSpawn = new(42.9f, 315.25f, -31f);

    private readonly static List<Pickup> WallItems = new();

    private readonly static List<uint> GivenWallItems = new();

    // These were all figured out by trial and error, someone with a model editor
    // and an affinity for figuring out the math to make these spawn perfectly in order
    // is free to do so
    private readonly static WallWeaponSpawn[] WeaponsToSpawn =
    {
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
    };

    private static void SpawnWorkbench()
    {
        Log.Debug("Instantiating bench.");

        var bench = PrefabHelper.Spawn(PrefabType.WorkstationStructure, BenchSpawnPosition, Quaternion.Euler(BenchSpawnRotation));

        if (bench is null)
        {
            Log.Error("Bench prefab not found, not spawning bench in tower!");
        }
    }

    private static void SpawnWallWeapons()
    {
        Log.Debug("Spawning tower wall weapons.");

        var yOffset = 0;
        var zOffset = 0;

        foreach (var spawn in WeaponsToSpawn)
        {
            var type = spawn.Type;
            var pickup = Pickup.Create(type);

            // Shouldn't matter as the weapons don't get picked up, but as a failsafe
            if (pickup is FirearmPickup weapon)
            {
                weapon.Ammo = 0;
            }

            pickup.PhysicsModule.Rb.isKinematic = true;
            WallItems.Add(pickup);

            pickup.Spawn(new Vector3(
                    InitialSpawn.x + spawn.Offset.x,
                    InitialSpawn.y + yOffset * WeaponSpawnYMargin + spawn.Offset.y,
                    InitialSpawn.z + zOffset * WeaponSpawnZMargin + spawn.Offset.z),
                Quaternion.Euler(spawn.Rotation)
            );

            yOffset++;

            if (yOffset >= WeaponsToSpawn.Length / 2)
            {
                zOffset++;
                yOffset = 0;
            }
        }
    }

    public static void OnPickingUpItem(PickingUpItemEventArgs args)
    {
        if (!WallItems.Contains(args.Pickup))
        {
            return;
        }

        var otherPickup = Pickup.Create(args.Pickup.Type);
        var itemInInventory = args.Player.AddItem(otherPickup);

        // Automatically equipping the weapon for the player ended up being too buggy
        // Left the reticle you usually see when you haven't equipped anything on
        // making changing optics annoying
        // args.Player.CurrentItem = itemInInventory;

        GivenWallItems.Add(itemInInventory.Serial);

        if (itemInInventory.IsWeapon && itemInInventory is Firearm weaponInInventory)
        {
            if (args.Player.Preferences.TryGetValue(itemInInventory.Type.GetFirearmType(), out var preferences))
            {
                weaponInInventory.AddAttachment(preferences);
            }

            weaponInInventory.BarrelAmmo = 0;
            weaponInInventory.MagazineAmmo = 0;
            weaponInInventory.MaxBarrelAmmo = 0;
            weaponInInventory.MaxMagazineAmmo = 0;
        }

        args.IsAllowed = false;
    }

    // Changes preference immediately if the player is a tutorial.
    public static void OnAttachmentChange(ChangingAttachmentsEventArgs args)
    {
        if (args.Player.Role != RoleTypeId.Tutorial)
        {
            return;
        }

        args.Firearm.AddPreference(args.Player, args.Firearm.FirearmType, args.NewAttachmentIdentifiers.ToArray());
    }

    public static void OnDroppingItem(DroppingItemEventArgs args)
    {
        if (!GivenWallItems.Contains(args.Item.Serial))
        {
            return;
        }

        args.Player.RemoveItem(args.Item);
        args.IsAllowed = false;
        GivenWallItems.Remove(args.Item.Serial);
    }

    public static void OnRoundStarted()
    {
        WallItems.Clear();
        GivenWallItems.Clear();
        SpawnWallWeapons();
        SpawnWorkbench();
    }

    private sealed class WallWeaponSpawn
    {
        public readonly Vector3 Offset;

        public readonly Vector3 Rotation;

        public readonly ItemType Type;

        public WallWeaponSpawn(ItemType weapon, Vector3 rotation, Vector3 offset)
        {
            Type = weapon;
            Rotation = rotation;
            Offset = offset;
        }
    }
}
