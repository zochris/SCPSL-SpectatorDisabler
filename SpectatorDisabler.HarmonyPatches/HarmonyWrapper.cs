using HarmonyLib;
using SpectatorDisabler.HarmonyPatches.Patches;

namespace SpectatorDisabler.HarmonyPatches;

public class HarmonyWrapper
{
    public HarmonyWrapper(string harmonyId, IHarmonyHelper helper)
    {
        HarmonyId = harmonyId;

        Scp1576WarningSendMessagePatch.Helper = helper;
    }

    private string HarmonyId { get; }

    private Harmony? HarmonyInstance { get; set; }

    public void Enable()
    {
        HarmonyInstance = new Harmony(HarmonyId);
        HarmonyInstance.PatchAll();
    }

    public void Disable()
    {
        HarmonyInstance?.UnpatchAll();
    }
}
