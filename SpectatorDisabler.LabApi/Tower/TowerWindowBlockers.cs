using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SpectatorDisabler.LabApi.Tower;

public class TowerWindowBlockers : CustomEventsHandler
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
        Logger.Debug("Spawning window blockers.");

        CreatePlane(TowerWindows1Pos, new Vector3(0, 0, 90), TowerPlaneScale);
        CreatePlane(TowerWindows1Pos, new Vector3(0, 0, -90), TowerPlaneScale);

        CreatePlane(TowerWindows2Pos, new Vector3(0, 0, 90), TowerPlaneScale);
        CreatePlane(TowerWindows2Pos, new Vector3(0, 0, -90), TowerPlaneScale);

        CreatePlane(TowerWindows3Pos, new Vector3(90, 0, 0), TowerPlaneScale);
        CreatePlane(TowerWindows3Pos, new Vector3(-90, 0, 0), TowerPlaneScale);

        CreatePlane(TowerWindows4Pos, new Vector3(90, 0, 0), TowerPlaneScale);
        CreatePlane(TowerWindows4Pos, new Vector3(-90, 0, 0), TowerPlaneScale);
    }

    private static void CreatePlane(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        var plane = PrimitiveObjectToy.Create(position, Quaternion.Euler(rotation), scale);
        plane.Type = PrimitiveType.Plane;
    }

    public override void OnServerRoundStarted()
    {
        base.OnServerRoundStarted();

        SpawnWindowBlockers();
    }
}
