using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Kitchen;
using Kitchen.Layouts;
using Kitchen.Layouts.Features;
using KitchenLib.Utils;
using StarsCookieJar.API;

namespace StarsCookieJar.Patches
{
    [HarmonyPatch(typeof(LayoutView), "UpdateData")]
    public class LayoutViewPatch
    {
        private static FieldInfo _Prefabs = ReflectionUtils.GetField<LayoutView>("Prefabs");
        static bool Prefix(LayoutView __instance, LayoutView.InitialViewData view_data)
        {
            if (__instance.IsInitialised) return true;
            
            LayoutPrefabSet prefab_override = null;
            foreach (Feature feature in view_data.Floorplan.Features)
            {
                if (CookieJarRegistry.PrefabOverrides.TryGetValue(feature.Type, out prefab_override))
                {
                    break;
                }
            }

            if (prefab_override == null)
            {
                return true;
            }

            __instance.IsInitialised = true;
            __instance.Builder = new Kitchen.LayoutBuilder(view_data.Floorplan, (LayoutPrefabSet)_Prefabs.GetValue(__instance), __instance.transform);
            
            Dictionary<string, object> CachedVariables = new Dictionary<string, object>();
            
            foreach (FieldInfo field in typeof(LayoutPrefabSet).GetFields())
            {
                var original = field.GetValue(__instance.Builder.Prefabs);
                var modified = field.GetValue(prefab_override);

                if (modified != null && original != modified)
                {
                    if (CachedVariables.TryAdd(field.Name, original))
                    {
                        field.SetValue(__instance.Builder.Prefabs, modified);
                    }
                }
            }

            LayoutMapGenerator.GenerateFor(view_data.Floorplan, true);
            __instance.Builder.Build();
            __instance.UpdateNavmesh();
            
            foreach (FieldInfo field in typeof(LayoutPrefabSet).GetFields())
            {
                if (CachedVariables.TryGetValue(field.Name, out object value))
                {
                    field.SetValue(__instance.Builder.Prefabs, value);
                }
            }
            
            return false;
        }
    }
}