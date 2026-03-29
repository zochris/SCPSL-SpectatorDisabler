using UnityEngine;

namespace SpectatorDisabler.TowerTools;

public class WallWeaponSpawn(ItemType weapon, Vector3 rotation, Vector3 offset)
{
    public readonly Vector3 Offset = offset;

    public readonly Vector3 Rotation = rotation;

    public readonly ItemType Type = weapon;
}
