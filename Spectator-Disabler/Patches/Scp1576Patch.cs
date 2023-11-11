using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features.Pools;
using HarmonyLib;
using InventorySystem.Items.Usables.Scp1576;
using JetBrains.Annotations;
using PlayerRoles;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch]
    internal static class Scp1576Patch
    {
        [UsedImplicitly]
        private static IEnumerable<MethodBase> TargetMethods()
        {
            // The check in Scp1576SpectatorWarningHandler.SendMessage is in a lambda expression.
            // So we have to target the compiler generated class.
            var originalUpdateType = AccessTools.Inner(typeof(Scp1576SpectatorWarningHandler), "<>c");
            var originalUpdateMoveNextMethod = AccessTools.Method(originalUpdateType, "<SendMessage>b__10_0");

            yield return originalUpdateMoveNextMethod;
        }

        /// <summary>
        ///     This transpiler replaces this:
        ///     <code>
        ///         return referenceHub.roleManager.CurrentRole is SpectatorRole;
        ///     </code>
        ///     With this:
        ///     <code>
        ///         return referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial
        ///     </code>
        /// </summary>
        /// <param name="instructions">
        ///     The <see cref="CodeInstruction" />s of the original
        ///     <see cref="Scp1576SpectatorWarningHandler.SendMessage" /> method.
        /// </param>
        /// <returns></returns>
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt);

            newInstructions.RemoveRange(index, 5);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Callvirt,
                                    AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, 14),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ret)
            });

            foreach (var instruction in newInstructions)
            {
                yield return instruction;
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
