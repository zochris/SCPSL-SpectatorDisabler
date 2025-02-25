using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.Voice;
using VoiceChat;

namespace SpectatorDisabler.Patches;

[HarmonyPatch(typeof(HumanVoiceModule), nameof(HumanVoiceModule.ValidateReceive))]
internal static class HumanVoiceModuleValidateReceivePatch
{
    /// <summary>
    ///     This Transpiler adds the following code to the beginning of the <see cref="HumanVoiceModule.ValidateReceive" />
    ///     method.
    ///     <code>
    ///         if (this.Owner.GetRoleId() == RoleTypeId.Tutorial && channel == VoiceChatChannel.Scp1576)
    ///         {
    ///             return VoiceChatChannel.Scp1576;
    ///         }
    /// 
    ///         if (this.Owner.GetRoleId() == RoleTypeId.Tutorial && channel == VoiceChatChannel.Spectator)
    ///         {
    ///             return VoiceChatChannel.Proximity;
    ///         }
    ///     </code>
    /// </summary>
    /// <param name="instructions"></param>
    /// <param name="generator"></param>
    /// <returns></returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var tutorialChatCheckLabel = generator.DefineLabel();

        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher
            .MatchStartForward()
            .CreateLabel(out var originalCode)
            .InsertAndAdvance(
                // Allows Tutorials to listen to Scp1576 channel
                // (this.Owner.GetRoleId() == RoleTypeId.Tutorial &&
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.Owner))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetRoleId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, tutorialChatCheckLabel),

                // channel == VoiceChatChannel.Scp1576)
                new CodeInstruction(OpCodes.Ldarg_2), // channel
                new CodeInstruction(OpCodes.Ldc_I4_S, (int)VoiceChatChannel.Scp1576),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, tutorialChatCheckLabel),

                // return VoiceChatChannel.Scp1576
                new CodeInstruction(OpCodes.Ldc_I4_S, (int)VoiceChatChannel.Scp1576),
                new CodeInstruction(OpCodes.Ret)
            )
            .InsertAndAdvance(
                // Allows Tutorials to talk to each other with proximity chat
                // (this.Owner.GetRoleId() == RoleTypeId.Tutorial &&
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(tutorialChatCheckLabel), // this
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.Owner))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetRoleId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, originalCode),

                // channel == VoiceChatChannel.Spectator)
                new CodeInstruction(OpCodes.Ldarg_2), // channel
                new CodeInstruction(OpCodes.Ldc_I4_S, (int)VoiceChatChannel.Spectator),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, originalCode),

                // return VoiceChatChannel.Proximity
                new CodeInstruction(OpCodes.Ldc_I4_S, (int)VoiceChatChannel.Proximity),
                new CodeInstruction(OpCodes.Ret)
            );

        return codeMatcher.Instructions();
    }
}
