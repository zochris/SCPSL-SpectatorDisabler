using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;

namespace SpectatorDisabler.Patches;

[HarmonyPatch(typeof(Scp106PocketItemManager), nameof(Scp106PocketItemManager.GetRandomValidSpawnPosition))]
internal static class PocketItemManagerPatch
{
    /// <summary>
    ///     This transpiler replaces the following code:
    ///     <code>
    ///         if (allHub.roleManager.CurrentRole is IFpcRole currentRole)
    ///         {
    ///             // get position to spawn item
    ///         }
    ///     </code>
    ///     With this one:
    ///     <code>
    ///         if (allHub.roleManager.CurrentRole is IFpcRole currentRole
    ///             && allHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Tutorial)
    ///         {
    ///             // get position to spawn item
    ///         }
    ///     </code>
    /// </summary>
    /// <param name="instructions">
    ///     The <see cref="CodeInstruction" />s of the original
    ///     <see cref="Scp106PocketItemManager.GetRandomValidSpawnPosition" /> method.
    /// </param>
    /// <returns>
    ///     The new patched <see cref="CodeInstruction" />s of the
    ///     <see cref="Scp106PocketItemManager.GetRandomValidSpawnPosition" />
    ///     method.
    /// </returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            .MatchStartForward(
                new CodeMatch(OpCodes.Isinst),
                new CodeMatch(OpCodes.Stloc_3),
                new CodeMatch(OpCodes.Ldloc_3)
            )
            // duplicate value of roleManager.CurrentRole on stack to use later
            .InsertAndAdvance(new CodeInstruction(OpCodes.Dup))
            .MatchStartForward(
                new CodeMatch(OpCodes.Brfalse_S),
                new CodeMatch(OpCodes.Ldloc_3),
                new CodeMatch(OpCodes.Callvirt)
            );

        // save operand of Brfalse_S
        var loopContinueOperand = codeMatcher.Operand;

        codeMatcher
            .Advance(1) // move over Brfalse_S
            .InsertAndAdvance(
                // add check for tutorial
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq), // currentRole.RoleTypeId == RoleTypeId.Tutorial
                new CodeInstruction(OpCodes.Brtrue_S, loopContinueOperand) // continue
            );

        return codeMatcher.Instructions();
    }
}
