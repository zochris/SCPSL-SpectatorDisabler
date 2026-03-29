using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Scp1509;
using JetBrains.Annotations;
using PlayerRoles;

namespace SpectatorDisabler.HarmonyPatches.Patches;

[HarmonyPatch(typeof(Scp1509RespawnEligibility), nameof(Scp1509RespawnEligibility.OnServerRoleSet))]
internal static class Scp1509RespawnEligibilityOnServerRoleSetPatch
{
    /// <summary>
    ///     Inserts the following check into <see cref="Scp1509RespawnEligibility.OnServerRoleSet" /> to skip updating the
    ///     previous role for Tutorials:
    ///     <code>
    ///         if (roleId == RoleTypeId.Tutorial)
    ///         {
    ///             return;
    ///         }
    ///     </code>
    /// </summary>
    /// <param name="instructions"></param>
    /// <param name="generator"></param>
    /// <returns></returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher
            .MatchEndForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Stloc_0),
                new CodeMatch(OpCodes.Ldsfld) // insert before this
            )
            .CreateLabel(out var originalCode)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, originalCode),
                new CodeInstruction(OpCodes.Ret)
            );

        return codeMatcher.Instructions();
    }
}
