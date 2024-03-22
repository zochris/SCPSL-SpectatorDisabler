using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch]
    internal static class Scp049OnServerRoleSetPatch
    {
        [UsedImplicitly]
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var generatedFunctions = AccessTools.Inner(typeof(Scp049ResurrectAbility), "<>c");
            var lambdaFunction = AccessTools.Method(generatedFunctions, "<Init>b__21_0");

            yield return lambdaFunction;
        }

        /// <summary>
        ///     This transpiler adds the following condition:
        ///     <code>if (newRole == RoleTypeId.Tutorial)</code>
        ///     <code>  return;</code>
        ///     to the lambda function that gets called when PlayerRoleManager.OnServerRoleSet gets fired.
        ///     This means that changing to tutorial does not reset the counter of how many times a player 
        ///     has been resurrected in their life.
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
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label jumpLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_S, 14),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, jumpLabel),
                new CodeInstruction(OpCodes.Ret),
            });
            newInstructions[5].labels.Add(jumpLabel);


            foreach (var instruction in newInstructions)
            {
                yield return instruction;
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
