using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;

namespace SpectatorDisabler.Patches
{
    public class TeamRespawnEventPrefixPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToArray();

            for (var i = 0; i < codes.Length; i++)
                if (codes[i].opcode == OpCodes.Ldc_I4_2 && codes[i + 1].opcode == OpCodes.Ceq &&
                    codes[i + 3].opcode == OpCodes.Ceq)
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 14);
                else
                    yield return codes[i];
        }
    }
}
