using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SpectatorDisabler.TowerTools;
using UnityEngine;
using FirearmPickup = Exiled.API.Features.Pickups.FirearmPickup;
using Pickup = Exiled.API.Features.Pickups.Pickup;

namespace SpectatorDisabler.Tower;

internal class Workstation : WorkstationBase
{
    override protected void LogDebug(string message)
    {
        Log.Debug(message);
    }

    override protected void LogError(string message)
    {
        Log.Error(message);
    }

    override protected uint SpawnWallPickup(ItemType type, Vector3 position, Quaternion rotation)
    {
        var pickup = Pickup.Create(type);

        if (pickup is FirearmPickup weapon)
        {
            weapon.Ammo = 0;
        }

        pickup.PhysicsModule.Rb.isKinematic = true;
        pickup.Spawn(position, rotation);

        return pickup.Serial;
    }

    override protected void SpawnWorkbenchObject(Vector3 position, Quaternion rotation)
    {
        var bench = PrefabHelper.Spawn(PrefabType.WorkstationStructure, position, rotation);

        if (bench is null)
        {
            Log.Error("Bench prefab not found, not spawning bench in tower!");
        }
    }

    public void OnWaitingForPlayers()
    {
        Initialize();
    }

    public void OnPickingUpItem(PickingUpItemEventArgs args)
    {
        if (!IsWallItem(args.Pickup.Serial))
        {
            return;
        }

        args.IsAllowed = false;

        var otherPickup = Pickup.Create(args.Pickup.Type);
        var itemInInventory = args.Player.AddItem(otherPickup);

        TrackGivenItem(itemInInventory.Serial);

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

    public void OnDroppingItem(DroppingItemEventArgs args)
    {
        if (!IsGivenItem(args.Item.Serial))
        {
            return;
        }

        args.Player.RemoveItem(args.Item);
        args.IsAllowed = false;
        RemoveGivenItem(args.Item.Serial);
    }
}
