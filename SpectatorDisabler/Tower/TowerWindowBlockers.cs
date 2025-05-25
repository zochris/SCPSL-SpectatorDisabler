using Exiled.API.Features;
using Exiled.API.Features.Toys;
using UnityEngine;

namespace SpectatorDisabler.Tower;

internal static class TowerWindowBlockers
{
    private readonly static Vector3 TowerWindows1Pos = new(34.91f, 314.75f, -33.35f);

    private readonly static Vector3 TowerWindows2Pos = new(34.91f, 314.75f, -30.75f);

    private readonly static Vector3 TowerWindows3Pos = new(37.8f, 314.75f, -36f);

    private readonly static Vector3 TowerWindows4Pos = new(40.4f, 314.75f, -36f);

    private readonly static Vector3 TowerPlaneScale = new(0.25f, 0.25f, 0.25f);

    // Because the plane primitive is a one way we spawn two facing both directions
    // This could technically be configured to only spawn one window and allow
    // Tutorial to look out.
    private static void SpawnWindowBlockers()
    {
        Log.Debug("Spawning window blockers.");

        Primitive.Create(PrimitiveType.Plane, TowerWindows1Pos, new Vector3(0, 0, 90), TowerPlaneScale);
        Primitive.Create(PrimitiveType.Plane, TowerWindows1Pos, new Vector3(0, 0, -90), TowerPlaneScale);

        Primitive.Create(PrimitiveType.Plane, TowerWindows2Pos, new Vector3(0, 0, 90), TowerPlaneScale);
        Primitive.Create(PrimitiveType.Plane, TowerWindows2Pos, new Vector3(0, 0, -90), TowerPlaneScale);

        Primitive.Create(PrimitiveType.Plane, TowerWindows3Pos, new Vector3(90, 0, 0), TowerPlaneScale);
        Primitive.Create(PrimitiveType.Plane, TowerWindows3Pos, new Vector3(-90, 0, 0), TowerPlaneScale);

        Primitive.Create(PrimitiveType.Plane, TowerWindows4Pos, new Vector3(90, 0, 0), TowerPlaneScale);
        Primitive.Create(PrimitiveType.Plane, TowerWindows4Pos, new Vector3(-90, 0, 0), TowerPlaneScale);
    }

    public static void OnRoundStarted()
    {
        SpawnWindowBlockers();
    }
}
