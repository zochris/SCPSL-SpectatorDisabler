using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayableScps;
using Respawning;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch]
    internal class ReplaceSpectatorWithTutorialPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            // Fix SCP049's ability to revive dead players
            yield return typeof(Scp049).GetMethod(nameof(Scp049.BodyCmd_ByteAndGameObject));

            // Patch drawing of new team to spawn to search for tutorials
            yield return typeof(RespawnTickets).GetMethod(nameof(RespawnTickets.DrawRandomTeam));

            // Patch spawning of new team to search for tutorials
            var firstInner = AccessTools.FirstInner(typeof(RespawnManager), type => type.Name.Contains("<>c"));
            var method = AccessTools.FirstMethod(firstInner, info => info.Name.Contains("b__16_0"));
            yield return method;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Replace check for RoleType.Spectator with check for RoleType.Tutorial
            var codes = instructions.ToArray();

            for (var i = 0; i < codes.Length; i++)
                if (codes[i].opcode == OpCodes.Ldc_I4_2 && codes[i - 1].opcode == OpCodes.Ldfld &&
                    codes[i - 2].opcode == OpCodes.Ldfld)
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 14);
                else
                    yield return codes[i];
        }
    }
}
