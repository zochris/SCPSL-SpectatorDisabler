using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Usables.Scp1576;
using JetBrains.Annotations;
using PlayerRoles;

namespace SpectatorDisabler.Patches;

[HarmonyPatch(typeof(Scp1576SpectatorWarningHandler), nameof(Scp1576SpectatorWarningHandler.SendMessage))]
internal static class Scp1576WarningSendMessagePatch
{
    public static void BroadcastWarningMessage(bool isStopping)
    {
        var tutorials = Player.List.Where(player => player.ReferenceHub.GetRoleId() == RoleTypeId.Tutorial);

        foreach (var tutorial in tutorials)
        {
            tutorial.Broadcast(5, isStopping ? "SCP-1576 is finished" : "SCP-1576 is about to be used");
        }
    }

    /// <summary>
    ///     This Transpiler adds a call to <see cref="BroadcastWarningMessage" /> to the top of
    ///     <see cref="Scp1576SpectatorWarningHandler.SendMessage" />.
    /// </summary>
    /// <param name="instructions"></param>
    /// <returns></returns>
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            .MatchStartForward()
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Scp1576WarningSendMessagePatch), nameof(BroadcastWarningMessage)))
            );

        return codeMatcher.Instructions();
    }
}
