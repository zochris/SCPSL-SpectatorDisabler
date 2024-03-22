using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Structs;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpectatorDisabler.Tower
{
    internal class TowerBench
    {
        static readonly Vector3 BENCH_SPAWN_POSITION = new Vector3(42.93f, 1013.05f, -34.55f);
        static readonly Vector3 BENCH_SPAWN_ROTATION = new Vector3(0f, -90f, 0f);

        static readonly Vector3 INITIAL_SPAWN = new Vector3(42.9f, 1015.25f, -31f);

        static readonly float WEAPON_SPAWN_Z_MARGIN = -1f;
        static readonly float WEAPON_SPAWN_Y_MARGIN = -0.35f;

        private static List<Pickup> _wallItems = new List<Pickup>();
        private static List<uint> _givenWallItems = new List<uint>();

        private sealed class WallWeaponSpawn
        {
            public ItemType type;
            public Vector3 rotation;
            public Vector3 offset;

            public WallWeaponSpawn(ItemType weapon, Vector3 rotation, Vector3 offset)
            {
                type = weapon;
                this.rotation = rotation;
                this.offset = offset;
            }
        }

        // These were all figured out by trial and error, someone with a model editor
        // and an affinity for figuring out the math to make these spawn perfectly in order
        // is free to do so
        private static readonly WallWeaponSpawn[] WEAPONS_TO_SPAWN = {
            new WallWeaponSpawn(ItemType.GunCOM15, new Vector3(0, 0, 0), new Vector3(0, -0.1f, 0)),
            new WallWeaponSpawn(ItemType.GunCOM18, new Vector3(0, 0, 0), new Vector3(-0.03f, 0, -0.1f)),
            new WallWeaponSpawn(ItemType.GunRevolver, new Vector3(0, 180, 0), new Vector3(0, 0, 0)),
            new WallWeaponSpawn(ItemType.GunFSP9, new Vector3(0, 180, 0), new Vector3(0, -0.02f, 0.1f)),
            new WallWeaponSpawn(ItemType.GunCrossvec, new Vector3(0, 180, 0), new Vector3(0, 0, 0)),
            new WallWeaponSpawn(ItemType.GunE11SR, new Vector3(0, 180, 0), new Vector3(0, -0.05f, 0.2f)),
            new WallWeaponSpawn(ItemType.GunAK, new Vector3(0, 0, 90), new Vector3(0, 0.07f, 0)),
            new WallWeaponSpawn(ItemType.GunShotgun, new Vector3(0, 0, 0), new Vector3(0, -0.1f, 0)),
            new WallWeaponSpawn(ItemType.GunFRMG0, new Vector3(0, 180, 0), new Vector3(0, -0.1f, 0)),
            new WallWeaponSpawn(ItemType.GunLogicer, new Vector3(0, -90, 0), new Vector3(0, 0, 0.1f)),
        };

        public static void SpawnWorkbench()
        {
            Log.Debug("Instantiating bench.");

            GameObject bench = NetworkClient.prefabs.Values.First(p => p.name.Contains("Work Station"));
            if (bench == null)
            {
                Log.Error("Bench prefab not found, not spawning bench in tower!");
                return;
            }

            bench = UnityEngine.Object.Instantiate(bench);
            if (!bench.TryGetComponent(out Transform t))
            {
                Log.Error("Could not get transform component of bench.");
                return;
            }

            NetworkServer.Spawn(bench);
            t.position = BENCH_SPAWN_POSITION;
            t.Rotate(BENCH_SPAWN_ROTATION);
        }

        public static void SpawnWallWeapons()
        {
            Log.Debug("Spawning tower wall weapons.");

            int yOffset = 0;
            int zOffset = 0;

            foreach (WallWeaponSpawn spawn in WEAPONS_TO_SPAWN)
            {
                ItemType type = spawn.type;
                Pickup l = Pickup.Create(type);

                // Shouldn't matter as the weapons don't get picked up, but as a failsafe
                FirearmPickup weapon = l as FirearmPickup;
                if (weapon != null)
                {
                    weapon.Ammo = 0;
                }

                l.PhysicsModule.Rb.isKinematic = true;
                _wallItems.Add(l);
                l.Spawn(new Vector3(
                        INITIAL_SPAWN.x + spawn.offset.x,
                        INITIAL_SPAWN.y + yOffset * WEAPON_SPAWN_Y_MARGIN + spawn.offset.y,
                        INITIAL_SPAWN.z + zOffset * WEAPON_SPAWN_Z_MARGIN + spawn.offset.z),
                    Quaternion.Euler(spawn.rotation)
                );

                yOffset++;
                if (yOffset >= 5)
                {
                    zOffset++;
                    yOffset = 0;
                }
            }
        }

        public static void OnPickingUpItem(PickingUpItemEventArgs args)
        {
            if (!_wallItems.Contains(args.Pickup))
            {
                return;
            }
            Pickup otherPickup = Pickup.Create(args.Pickup.Type);
            Item itemInInventory = args.Player.AddItem(otherPickup);

            // Automatically equipping the weapon for the player ended up being too buggy
            // Left the reticle you usually see when you haven't equipped anything on
            // making changing optics annoying
            // args.Player.CurrentItem = itemInInventory;

            _givenWallItems.Add(itemInInventory.Serial);
            if (itemInInventory.IsWeapon)
            {
                Firearm weaponInInventory = itemInInventory as Firearm;
                if (args.Player.Preferences.TryGetValue(itemInInventory.Type.GetFirearmType(), out AttachmentIdentifier[] preferences))
                {
                    weaponInInventory.AddAttachment(preferences);
                }
                weaponInInventory.Ammo = 0;
                weaponInInventory.MaxAmmo = 0;
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
            if (!_givenWallItems.Contains(args.Item.Serial))
            {
                return;
            }
            args.Player.RemoveItem(args.Item);
            args.IsAllowed = false;
            _givenWallItems.Remove(args.Item.Serial);
        }

        public static void OnRoundStarted()
        {
            _wallItems.Clear();
            _givenWallItems.Clear();
            SpawnWallWeapons();
            SpawnWorkbench();
        }
    }
}
