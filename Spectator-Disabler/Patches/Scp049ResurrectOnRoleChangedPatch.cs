using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features.Pools;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch]
    internal class Scp049ResurrectOnRoleChangedPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var generatedFunctions = AccessTools.Inner(typeof(Scp049ResurrectAbility), "<>c");
            var lambdaFunction = AccessTools.Method(generatedFunctions, "<Init>b__21_2");

            yield return lambdaFunction;
        }

        /// <summary>
        ///     This transpiler adds the following condition:
        ///     <code>if (newRole.RoleTypeId == RoleTypeId.Tutorial)</code>
        ///     <code>  return;</code>
        ///     to the lambda function that gets called when PlayerRoleManager.OnRoleChanged gets fired.
        ///     This means that the DeadZombies is not modified when changing to tutorial, allowing
        ///     SCP-049 to keep track of zombies that were just called.
        /// </summary>
        /// <param name="instructions">
        ///     The <see cref="CodeInstruction" />s of the original
        ///     <see cref="Scp049ResurrectAbility.Init" /> method.
        /// </param>
        /// <param name="generator">
        ///     An <see cref="ILGenerator" />s injected by Harmony to generate labels.
        /// </param>
        /// <returns>
        ///     The new patched <see cref="CodeInstruction" />s of the lambda function.
        /// </returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label jumpLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, 14),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, jumpLabel),
                new CodeInstruction(OpCodes.Ret),
            });
            // Add a jump to right after the ret
            newInstructions[6].labels.Add(jumpLabel);


            foreach (var instruction in newInstructions)
            {
                yield return instruction;
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
