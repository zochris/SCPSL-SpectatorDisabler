using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.IsSpawnableSpectator))]
    internal static class Scp049ResurrectCheckTargetPatch
    {
        /// <summary>
        ///     This transpiler replaces the following code:
        ///     <code>
        ///         if (hub.roleManager.CurrentRole is SpectatorRole spectatorRole)
        ///         {
        ///             return spectatorRole.ReadyToRespawn;
        ///         }
        ///         return false;
        ///     </code>
        ///     With this one:
        ///     <code>
        ///         return hub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial
        ///     </code>
        /// </summary>
        /// <param name="instructions">
        ///     The <see cref="CodeInstruction" />s of the original
        ///     <see cref="Scp049ResurrectAbility.IsSpawnableSpectator" /> method.
        /// </param>
        /// <returns>
        ///     The new patched <see cref="CodeInstruction" />s of the <see cref="Scp049ResurrectAbility.IsSpawnableSpectator" />
        ///     method.
        /// </returns>
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            codeMatcher
                .MatchStartForward(
                    new CodeMatch(OpCodes.Isinst),
                    new CodeMatch(OpCodes.Stloc_0),
                    new CodeMatch(OpCodes.Ldloc_0),
                    new CodeMatch(OpCodes.Brfalse_S)
                )
                .RemoveInstructions(4)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Ret)
                );

            return codeMatcher.Instructions();
        }
    }
}
