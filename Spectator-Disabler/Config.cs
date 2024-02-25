using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SpectatorDisabler
{
    public class Config : IConfig
    {
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enables debug logging")]
        public bool Debug { get; set; } = false;

        [Description("Enables blocking tower view with planes")]
        public bool TowerBlockView { get; set; } = true;
    }
}
