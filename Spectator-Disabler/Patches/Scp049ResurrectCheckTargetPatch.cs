using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features.Pools;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateAny))]
    internal static class Scp049ResurrectCheckTargetPatch
    {
        /// <summary>
        ///     This transpiler replaces the following condition:
        ///     <code>ownerHub.roleManager.CurrentRole is SpectatorRole</code>
        ///     With this one:
        ///     <code>ownerHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial</code>
        /// </summary>
        /// <param name="instructions">
        ///     The <see cref="CodeInstruction" />s of the original
        ///     <see cref="Scp049ResurrectAbility.ServerValidateAny" /> method.
        /// </param>
        /// <returns>
        ///     The new patched <see cref="CodeInstruction" />s of the <see cref="Scp049ResurrectAbility.ServerValidateAny" />
        ///     method.
        /// </returns>
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            for (var i = 0; i < newInstructions.Count; i++)
            {
                if (newInstructions[i].opcode == OpCodes.Isinst
                    && newInstructions[i - 1].opcode == OpCodes.Callvirt
                    && newInstructions[i - 2].opcode == OpCodes.Ldfld
                    && newInstructions[i - 3].opcode == OpCodes.Ldloc_0)
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_S, 14);
                    yield return new CodeInstruction(OpCodes.Ceq);
                }
                else
                {
                    yield return newInstructions[i];
                }
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
