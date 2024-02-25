using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Item;
using MEC;
using PlayerRoles;

namespace SpectatorDisabler
{
    public static class EventHandler
    {
        public static void OnPlayerSpawning(SpawnedEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Spectator)
            {
                Timing.CallDelayed(1, () => { ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint); });
            }

            if (ev.Reason == SpawnReason.Revived)
            {
                var scp = Player.List.FirstOrDefault(player => player.Role == RoleTypeId.Scp049);

                if (scp != null)
                {
                    ev.Player.Position = scp.Position;
                }
            }
        }

        public static void OnFinishingRecall(FinishingRecallEventArgs ev)
        {
            // This should default to true, but for some reason it does not
            ev.IsAllowed = true;
        }

        public static void OnRoundStarted()
        {
            TowerUtils.SpawnWindowBlockers();
            TowerUtils.SpawnWorkbench();
            TowerUtils.SpawnWallWeapons();
        }

        public static void OnPickingUpItem(PickingUpItemEventArgs args)
        {
            TowerUtils.OnPickingUpItem(args);
        }

        public static void OnAttachmentChange(ChangingAttachmentsEventArgs args)
        {
            TowerUtils.OnAttachmentChange(args);
        }

        public static void OnDroppingItem(DroppingItemEventArgs args)
        {
            TowerUtils.OnDroppingItem(args);
        }
    }
}
