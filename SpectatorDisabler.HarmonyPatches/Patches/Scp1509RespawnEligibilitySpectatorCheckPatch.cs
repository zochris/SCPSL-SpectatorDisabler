using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Scp1509;
using JetBrains.Annotations;
using PlayerRoles;

namespace SpectatorDisabler.HarmonyPatches.Patches;

[HarmonyPatch]
internal static class Scp1509RespawnEligibilitySpectatorCheckPatch
{
    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Scp1509RespawnEligibility), nameof(Scp1509RespawnEligibility.IsEligible));
        yield return AccessTools.Method(typeof(Scp1509RespawnEligibility), nameof(Scp1509RespawnEligibility.IsAnyEligibleSpectators));
        yield return AccessTools.Method(typeof(Scp1509RespawnEligibility), nameof(Scp1509RespawnEligibility.GetEligibleSpectator));
    }

    /// <summary>
    ///     Replaces the check for Spectators with a check for Tutorials in the following methods:
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="Scp1509RespawnEligibility.IsEligible" />
    ///         </item>
    ///         <item>
    ///             <see cref="Scp1509RespawnEligibility.IsAnyEligibleSpectators" />
    ///         </item>
    ///         <item>
    ///             <see cref="Scp1509RespawnEligibility.GetEligibleSpectator" />
    ///         </item>
    ///     </list>
    ///     Original check:
    ///     <code>
    ///         if (player.roleManager.CurrentRole is SpectatorRole currentRole &amp;&amp; currentRole.ReadyToRespawn &amp;&amp; ...)
    ///     </code>
    ///     Patched check:
    ///     <code>
    ///         if (player.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial &amp;&amp; ...)
    ///     </code>
    /// </summary>
    /// <param name="instructions"></param>
    /// <param name="generator"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher
            .MatchEndForward(
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Isinst) // remove starting here
            )
            .RemoveInstructions(6)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq)
            );

        return codeMatcher.Instructions();
    }
}
