using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features.Pools;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;

namespace SpectatorDisabler.Patches
{
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
        ///     The new patched <see cref="CodeInstruction" />s of the <see cref="Scp106PocketItemManager.GetRandomValidSpawnPosition" />
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
                    && newInstructions[i - 3].opcode == OpCodes.Call)
                {
                    // duplicate value of roleManager.CurrentRole on stack to use later
                    yield return new CodeInstruction(OpCodes.Dup);
                }

                if (newInstructions[i].opcode == OpCodes.Ldloc_3
                    && newInstructions[i - 1].opcode == OpCodes.Brfalse_S
                    && newInstructions[i - 2].opcode == OpCodes.Ldloc_3
                    && newInstructions[i - 3].opcode == OpCodes.Stloc_3)
                {
                    // add check for tutorial
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_S, 14); // RoleTypeId.Tutorial
                    yield return new CodeInstruction(OpCodes.Ceq); // currentRole.RoleTypeId == RoleTypeId.Tutorial
                    yield return new CodeInstruction(OpCodes.Brtrue_S, newInstructions[i - 1].operand); // continue
                }

                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
