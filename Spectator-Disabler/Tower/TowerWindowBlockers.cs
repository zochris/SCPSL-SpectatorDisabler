using Exiled.API.Features.Toys;
using UnityEngine;
using Exiled.API.Features;

namespace SpectatorDisabler.Tower
{
    internal static class TowerWindowBlockers
    {
        static readonly Vector3 TOWER_WINDOWS_1_POS = new Vector3(34.91f, 1014.75f, -33.35f);
        static readonly Vector3 TOWER_WINDOWS_2_POS = new Vector3(34.91f, 1014.75f, -30.75f);
        static readonly Vector3 TOWER_WINDOWS_3_POS = new Vector3(37.8f, 1014.75f, -36f);
        static readonly Vector3 TOWER_WINDOWS_4_POS = new Vector3(40.4f, 1014.75f, -36f);
        static readonly Vector3 TOWER_PLANE_SCALE = new Vector3(0.25f, 0.25f, 0.25f);

        // Because the plane primitive is a one way we spawn two facing both directions
        // This could technically be configured to only spawn one window and allow
        // Tutorial to look out.
        public static void SpawnWindowBlockers()
        {
            Log.Debug("Spawning window blockers.");

            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_1_POS, new Vector3(0, 0, 90), TOWER_PLANE_SCALE);
            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_1_POS, new Vector3(0, 0, -90), TOWER_PLANE_SCALE);

            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_2_POS, new Vector3(0, 0, 90), TOWER_PLANE_SCALE);
            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_2_POS, new Vector3(0, 0, -90), TOWER_PLANE_SCALE);

            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_3_POS, new Vector3(90, 0, 0), TOWER_PLANE_SCALE);
            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_3_POS, new Vector3(-90, 0, 0), TOWER_PLANE_SCALE);

            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_4_POS, new Vector3(90, 0, 0), TOWER_PLANE_SCALE);
            Primitive.Create(PrimitiveType.Plane, TOWER_WINDOWS_4_POS, new Vector3(-90, 0, 0), TOWER_PLANE_SCALE);
        }

        public static void OnRoundStarted()
        {
            SpawnWindowBlockers();
        }
    }
}
