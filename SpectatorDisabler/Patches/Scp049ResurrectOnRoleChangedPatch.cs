using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.Patches;

[HarmonyPatch]
internal static class Scp049ResurrectOnRoleChangedPatch
{
    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var generatedFunctions = AccessTools.Inner(typeof(Scp049ResurrectAbility), "<>c");
        var lambdaFunction = AccessTools.Method(generatedFunctions, "<Init>b__29_2");

        yield return lambdaFunction;
    }

    /// <summary>
    ///     This transpiler adds the following condition:
    ///     <code>
    ///         if (newRole.RoleTypeId == RoleTypeId.Tutorial)
    ///             return;
    ///     </code>
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
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher
            .MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_3),
                new CodeMatch(OpCodes.Isinst),
                new CodeMatch(OpCodes.Brfalse_S)
            )
            .RemoveInstructions(2)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq)
            );

        return codeMatcher.Instructions();
    }
}
