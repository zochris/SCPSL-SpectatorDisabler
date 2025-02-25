using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.Voice;
using VoiceChat;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch(typeof(HumanVoiceModule), nameof(HumanVoiceModule.ValidateSend))]
    internal static class HumanVoiceModuleValidateSendPatch
    {
        /// <summary>
        ///     This Transpiler adds the following code to the beginning of the <see cref="HumanVoiceModule.ValidateSend" /> method.
        ///     <code>
        ///         if (this.Owner.GetRoleId() == RoleTypeId.Tutorial)
        ///         {
        ///             return VoiceChatChannel.Spectator;
        ///         }
        ///     </code>
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codeMatcher = new CodeMatcher(instructions, generator);

            codeMatcher.MatchStartForward()
                .CreateLabel(out var returnLabel)
                .InsertAndAdvance(
                    // this.Owner.GetRoleId() == RoleTypeId.Tutorial
                    new CodeInstruction(OpCodes.Ldarg_0), // this
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.Owner))),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetRoleId))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleTypeId.Tutorial),
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Brfalse_S, returnLabel), // jump to original function

                    // return VoiceChatChannel.Spectator
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int)VoiceChatChannel.Spectator),
                    new CodeInstruction(OpCodes.Ret)
                );

            return codeMatcher.Instructions();
        }
    }
}
