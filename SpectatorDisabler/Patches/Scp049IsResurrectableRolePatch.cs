using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.Patches;

[HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.IsResurrectableRole))]
internal static class Scp049IsResurrectableRolePatch
{
    /// <summary>
    ///     This transpiler replaces the following check:
    ///     <code>
    ///         if (!role.IsFlamingo())
    ///         {
    ///             return role.IsHuman();
    ///         }
    ///     </code>
    ///     with this check:
    ///     <code>
    ///         if (!role.IsFlamingo())
    ///         {
    ///             return role == RoleTypeId.Tutorial;
    ///         }
    ///     </code>
    /// </summary>
    /// <param name="instructions">The original instructions.</param>
    /// <returns>The patched instructions.</returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            .MatchStartForward(
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Ret)
            )
            .RemoveInstruction()
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq)
            );

        return codeMatcher.Instructions();
    }
}
