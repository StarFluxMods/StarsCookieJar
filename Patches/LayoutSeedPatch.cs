using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Kitchen;
using KitchenData;
using StarsCookieJar.API;
using Random = UnityEngine.Random;

namespace StarsCookieJar.Patches
{
    /*
     * When a RestaurantSetting is registered with one or more LayoutProfiles, and doesn't have a `ForcedLayout`, this patch is designed to select a random LayoutProfile to use
     */
    [HarmonyPatch]
    public class LayoutSeedPatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            Type type = typeof(LayoutSeed);
            return AccessTools.FirstMethod(type, method => method.Name.Contains("GenerateMap"));
        }
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            CodeMatcher matcher = new(instructions);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_1), new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Call) , new CodeMatch(OpCodes.Ceq), new CodeMatch(OpCodes.Stloc_2))
                .Advance(5)
                .Insert(new CodeInstruction(OpCodes.Stloc_1))
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LayoutSeedPatch), "RestaurantSettingForcedLayout")))
                .Insert(new CodeInstruction(OpCodes.Ldloc_1))
                .Insert(new CodeInstruction(OpCodes.Ldloc_0))
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(LayoutSeed), "FixedSeed")))
                .Insert(new CodeInstruction(OpCodes.Ldarg_0));
            return matcher.InstructionEnumeration();
        }
        
        public static int RestaurantSettingForcedLayout(Seed FixedSeedContext, RestaurantSetting setting, int existingNumber)
        {
            if (setting.ForceLayout != null) return setting.ForceLayout.ID;

            using (FixedSeedContext fixedSeedContext = new FixedSeedContext(FixedSeedContext, 98234234))
            {
                using (fixedSeedContext.UseSubcontext(0))
                {
                    if (CookieJarRegistry._settingLayoutPairs.ContainsKey(setting.ID) && CookieJarRegistry._settingLayoutPairs[setting.ID].Count > 0)
                    {
                        return CookieJarRegistry._settingLayoutPairs[setting.ID][Random.Range(0, CookieJarRegistry._settingLayoutPairs[setting.ID].Count - 1)];
                    }
                }
            }

            return existingNumber;
        }
    }
}