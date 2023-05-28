using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features.Pools;
using HarmonyLib;
using PlayerRoles;
using Respawning;

namespace SpectatorDisabler.Patches
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.CheckSpawnable))]
    internal class RespawnManagerPatch
    {
        // Replace this:
        // if (ply.roleManager.CurrentRole is SpectatorRole spectatorRole)
        // {
        //     return spectatorRole.ReadyToRespawn;
        // }
        // return false;

        // With this:
        // return ply.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Tutorial
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -3;
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.RemoveRange(index, 9);

            newInstructions.InsertRange(index,
                                        new[]
                                        {
                                            new CodeInstruction(OpCodes.Callvirt,
                                                                AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                                            new CodeInstruction(OpCodes.Ldc_I4_S, 14),
                                            new CodeInstruction(OpCodes.Ceq),
                                            new CodeInstruction(OpCodes.Ret)
                                        });

            foreach (var instruction in newInstructions)
            {
                yield return instruction;
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
