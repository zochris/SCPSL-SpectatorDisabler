using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;

namespace SpectatorDisabler.HarmonyPatches.Patches;

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
    ///     This repaces the following condition in the lambda function that gets called when PlayerRoleManager.OnRoleChanged
    ///     gets fired:
    ///     <code>
    ///         if (prevRole is ZombieRole &amp;&amp; newRole is SpectatorRole)
    ///     </code>
    ///     With the following:
    ///     <code>
    ///         if (prevRole is ZombieRole &amp;&amp; newRole.RoleTypeId == RoleTypeId.Tutorial)
    ///     </code>
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
            .MatchEndForward(
                new CodeMatch(OpCodes.Ldarg_2),
                new CodeMatch(OpCodes.Isinst),
                new CodeMatch(OpCodes.Brfalse_S),
                new CodeMatch(OpCodes.Ldarg_3)
            )
            .Advance(1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq)
            )
            .RemoveInstruction();

        return codeMatcher.Instructions();
    }
}
