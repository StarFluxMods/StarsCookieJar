using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Kitchen;
using StarsCookieJar.Systems;
using Unity.Entities;

namespace StarsCookieJar.Patches
{
    /*
     * This patch is checking what blueprints are getting rerolled, and if that blueprint is a specialty decoration it will prevent it from being rerolled.
     */
    [HarmonyPatch]
    public class HandleShopRerollPatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            Type type = typeof(HandleShopReroll);
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OnUpdate"));
        }
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            CodeMatcher matcher = new(instructions);
            Label skipLabel = il.DefineLabel();

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloca_S), new CodeMatch(OpCodes.Call), new CodeMatch(OpCodes.Stloc_3)
                    , new CodeMatch(OpCodes.Br), new CodeMatch(OpCodes.Ldloca_S), new CodeMatch(OpCodes.Call), new CodeMatch(OpCodes.Stloc_S))
                .Advance(7)
                .Insert(new CodeInstruction(OpCodes.Brtrue, skipLabel))
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HandleShopRerollPatch), "DoesEntityHaveComponent")))
                .Insert(new CodeInstruction(OpCodes.Ldloc_S, 4))
                .MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EntityManager), "DestroyEntity", [typeof(Entity)])))
                .Advance(1)
                .Labels.Add(skipLabel);
            return matcher.InstructionEnumeration();
        }
        
        public static bool DoesEntityHaveComponent(Entity entity)
        {
            return HelperSystem.Instance.DoesEntityHaveComponent(entity);
        }
    }
}