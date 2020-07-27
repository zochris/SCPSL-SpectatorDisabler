using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SpectatorDisabler
{
    public  class Config : IConfig
    {
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates wether the custom remaining targets message is shown")]
        public bool ShowRemainingTargets { get; set; } = true;
    }
}
