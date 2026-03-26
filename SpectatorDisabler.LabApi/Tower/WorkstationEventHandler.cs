using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SpectatorDisabler.LabApi.Tower;

public class WorkstationEventHandler : CustomEventsHandler
{
    private readonly Workstation _workstation = new();

    public override void OnServerWaitingForPlayers()
    {
        base.OnServerWaitingForPlayers();
        _workstation.Initialize();
    }

    public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
    {
        base.OnPlayerDroppingItem(ev);

        if (!_workstation.IsGivenItem(ev.Item.Serial))
        {
            return;
        }

        ev.Player.RemoveItem(ev.Item);
        ev.IsAllowed = false;
        _workstation.RemoveGivenItem(ev.Item.Serial);
    }

    public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
    {
        base.OnPlayerPickingUpItem(ev);

        if (!_workstation.IsWallItem(ev.Pickup.Serial))
        {
            return;
        }

        ev.IsAllowed = false;

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

        _workstation.TrackGivenItem(itemInInventory.Serial);

        if (itemInInventory.Base is Firearm weaponInInventory
            && AttachmentsServerHandler.PlayerPreferences.TryGetValue(ev.Player.ReferenceHub, out var preferences)
            && preferences.TryGetValue(itemInInventory.Type, out var items))
        {
            weaponInInventory.ApplyAttachmentsCode(items, true);
        }
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
}
