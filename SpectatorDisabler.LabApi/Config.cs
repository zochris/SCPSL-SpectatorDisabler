using System;
using System.ComponentModel;

namespace SpectatorDisabler.LabApi;

[Serializable]
public class Config
{
    [Description("Enables blocking the windows looking out to the surface in the tower.")]
    public bool TowerWindowBlockers { get; set; } = false;

    [Description("Enables spawning a workbench and weapons to change preferences in tower.")]
    public bool TowerWorkbench { get; set; } = true;
}
