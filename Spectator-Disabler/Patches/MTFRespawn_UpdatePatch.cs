using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;

namespace SpectatorDisabler.Patches
{
    public class MTFRespawn_UpdatePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Dup)
				{
					if (codes[i + 1].opcode == OpCodes.Stsfld)
					{
                        if (codes[i + 2].opcode == OpCodes.Call && codes[i + 3].opcode == OpCodes.Brfalse)
                        {
                            codes[i + 3].opcode = OpCodes.Pop;
                            codes[i + 3].operand = null;

							i += 2;
                        }
                    }
                }
            }

            return codes.AsEnumerable();
        }
    }
}
