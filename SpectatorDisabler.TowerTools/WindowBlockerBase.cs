using UnityEngine;

namespace SpectatorDisabler.TowerTools;

public abstract class WindowBlockerBase
{
    private readonly static Vector3 TowerWindows1Pos = new(34.91f, 314.75f, -33.35f);

    private readonly static Vector3 TowerWindows2Pos = new(34.91f, 314.75f, -30.75f);

    private readonly static Vector3 TowerWindows3Pos = new(37.8f, 314.75f, -36f);

    private readonly static Vector3 TowerWindows4Pos = new(40.4f, 314.75f, -36f);

    private readonly static Vector3 TowerPlaneScale = new(0.25f, 0.25f, 0.25f);

    protected abstract void LogDebug(string message);

    protected abstract void CreatePlane(Vector3 position, Vector3 eulerRotation, Vector3 scale);

    // Because the plane primitive is one-way we spawn two facing both directions.
    // This could technically be configured to only spawn one window and allow
    // Tutorial to look out.
    public void SpawnWindowBlockers()
    {
        LogDebug("Spawning window blockers.");

        CreatePlane(TowerWindows1Pos, new Vector3(0, 0, 90), TowerPlaneScale);
        CreatePlane(TowerWindows1Pos, new Vector3(0, 0, -90), TowerPlaneScale);

        CreatePlane(TowerWindows2Pos, new Vector3(0, 0, 90), TowerPlaneScale);
        CreatePlane(TowerWindows2Pos, new Vector3(0, 0, -90), TowerPlaneScale);

        CreatePlane(TowerWindows3Pos, new Vector3(90, 0, 0), TowerPlaneScale);
        CreatePlane(TowerWindows3Pos, new Vector3(-90, 0, 0), TowerPlaneScale);

        CreatePlane(TowerWindows4Pos, new Vector3(90, 0, 0), TowerPlaneScale);
        CreatePlane(TowerWindows4Pos, new Vector3(-90, 0, 0), TowerPlaneScale);
    }
}
