using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SpectatorDisabler;

public class Config : IConfig
{
    [Description("Enables blocking the windows looking out to the surface in the tower.")]
    public bool TowerWindowBlockers { get; set; } = false;

    [Description("Enables spawning a workbench and weapons to change preferences in tower.")]
    public bool TowerWorkbench { get; set; } = true;

    [Description("Indicates whether the plugin is enabled or not")]
    public bool IsEnabled { get; set; } = true;

    [Description("Enables debug logging")]
    public bool Debug { get; set; } = false;
}
