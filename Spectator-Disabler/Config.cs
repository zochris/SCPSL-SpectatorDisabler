using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SpectatorDisabler
{
    public class Config : IConfig
    {
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates wether the custom remaining targets message is shown")]
        public bool ShowRemainingTargetsMessage { get; set; } = true;

        [Description("How long the remaining targets message should be shown")]
        public ushort RemainingTargetsMessageDuration { get; set; } = 5;
    }
}
