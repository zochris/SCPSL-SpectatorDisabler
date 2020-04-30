using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SpectatorDisabler.Patches
{
    class Scp049PlayerScriptCallCmdRecallPlayerPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_2 && codes[i + 1].opcode == OpCodes.Bne_Un)
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 14);
                else
                    yield return codes[i];
            }
        }
    }
}
