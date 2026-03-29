using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles.Voice;
using VoiceChat.Networking;

namespace SpectatorDisabler.HarmonyPatches.Patches;

[HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
internal static class VoiceTransceiverServerReceiveMessagePatch
{
    /// <summary>
    ///     Adds the following code to <see cref="VoiceTransceiver.ServerReceiveMessage" /> after the
    ///     <see cref="HumanVoiceModule.ValidateSend" /> call.
    ///     <code>
    ///         msg.Channel = channel;
    ///     </code>
    /// </summary>
    /// <param name="instructions"></param>
    /// <param name="generator"></param>
    /// <returns></returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher
            .MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldloc_1)
            )
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarga_S, 1), // msg
                new CodeInstruction(OpCodes.Ldloc_1), // channel
                new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(VoiceMessage), nameof(VoiceMessage.Channel))) // msg.Channel = channel
            );

        return codeMatcher.Instructions();
    }
}
