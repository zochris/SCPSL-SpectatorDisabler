using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch(typeof(Player), "get_IsDead")]
    internal class PlayerGet_IsDeadPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Replace check for Team.RIP with check for Team.TUT
            var codes = instructions.ToList();

            foreach (var instruction in codes)
                if (instruction.opcode == OpCodes.Ldc_I4_5)
                    yield return new CodeInstruction(OpCodes.Ldc_I4_6);
                else
                    yield return instruction;
        }
    }
}
