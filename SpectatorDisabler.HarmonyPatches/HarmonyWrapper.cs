using HarmonyLib;
using SpectatorDisabler.HarmonyPatches.Patches;

namespace SpectatorDisabler.HarmonyPatches;

public class HarmonyWrapper
{
    private string HarmonyId { get; }
    private Harmony? HarmonyInstance { get; set; }

    public HarmonyWrapper(string harmonyId, IHarmonyHelper helper)
    {
        HarmonyId = harmonyId;

        Scp1576WarningSendMessagePatch.Helper = helper;

        // FIXME: this is only for testing
        ServerValidateBegin.Helper = helper;
        CheckRagdoll.Helper = helper;
        CheckBeginConditions.Helper = helper;
        IsResurrectableRole.Helper = helper;
    }

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
