using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using Respawning.Waves;

namespace SpectatorDisabler.Patches;

[HarmonyPatch(typeof(WaveSpawner), nameof(WaveSpawner.CanBeSpawned))]
internal static class WaveSpawnerPatch
{
    /// <summary>
    ///     This transpiler replaces this:
    ///     <code>
    ///         if (ply.roleManager.CurrentRole is SpectatorRole spectatorRole)
    ///         {
    ///             return spectatorRole.ReadyToRespawn;
    ///         }
    ///         return false;
    ///     </code>
    ///     With this:
    ///     <code>
    ///         return ply.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial
    ///     </code>
    /// </summary>
    /// <param name="instructions">
    ///     The <see cref="CodeInstruction" />s of the original
    ///     <see cref="WaveSpawner.CanBeSpawned" /> method.
    /// </param>
    /// <returns>The new patched <see cref="CodeInstruction" />s of the <see cref="WaveSpawner.CanBeSpawned" /> method.</returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            .MatchStartForward(new CodeMatch(OpCodes.Isinst))
            .RemoveInstructions(codeMatcher.Remaining)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ret));

        return codeMatcher.Instructions();
    }
}
