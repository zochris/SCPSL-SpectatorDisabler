using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
            var lambdaFunction = AccessTools.Method(generatedFunctions, "<Init>b__29_0");

            yield return lambdaFunction;
        }

        /// <summary>
        ///     This transpiler adds the following condition:
        ///     <code>
        ///         if (newRole == RoleTypeId.Tutorial)
        ///             return;
        ///     </code>
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
            var codeMatcher = new CodeMatcher(instructions, generator);

            codeMatcher
                .MatchStartForward()
                .CreateLabel(out var originalCode)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Brfalse, originalCode),
                    new CodeInstruction(OpCodes.Ret)
                );

            return codeMatcher.Instructions();
        }
    }
}
