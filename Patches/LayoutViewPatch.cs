using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Kitchen;
using Kitchen.Layouts;
using Kitchen.Layouts.Features;
using KitchenLib.Utils;
using StarsCookieJar.API;
using UnityEngine;

namespace StarsCookieJar.Patches
{
    /*
     * This patch is designed to allow custom LayoutPrefabSets to be used when generating
     * The layout will need a `TypeMarker` node to identify it, and will need a `LayoutPrefabSet` registerd with `CookieJarRegistry.RegisterCustomPrefabSet()`
     */
    [HarmonyPatch(typeof(LayoutView), "UpdateData")]
    public class LayoutViewPatch
    {
        private static Dictionary<FeatureType, LayoutPrefabSet> PrefabSets = new Dictionary<FeatureType, LayoutPrefabSet>();
        
        private static FieldInfo _Prefabs = ReflectionUtils.GetField<LayoutView>("Prefabs");
        static bool Prefix(LayoutView __instance, LayoutView.InitialViewData view_data)
        {
            if (__instance.IsInitialised) // If the layout has already been initialised, skip, we're too late anyway 
            {
                return true;
            }
            
            // Check the layout in question for any registered markers.
            LayoutPrefabSet prefabSetOverride = null;
            Feature prefabSetMarker = null;
            foreach (Feature feature in view_data.Floorplan.Features)
            {
                if (CookieJarRegistry.PrefabOverrides.TryGetValue(feature.Type, out prefabSetOverride))
                {
                    prefabSetMarker = feature;
                    break;
                }
            }
            
            // If a marker was found, check if there was an override set attached to it, otherwise fail.
            if (prefabSetOverride == null)
            {
                return true;
            }

            // Create a cache set not to affect other settings, and apply the override prefabs to it.
            if (!PrefabSets.ContainsKey(prefabSetMarker.Type))
            {
                LayoutPrefabSet newPrefabSet = ScriptableObject.CreateInstance<LayoutPrefabSet>();
                LayoutPrefabSet originalPrefabSet = (LayoutPrefabSet)_Prefabs.GetValue(__instance);

                foreach (FieldInfo variable in typeof(LayoutPrefabSet).GetFields())
                {
                    variable.SetValue(newPrefabSet, variable.GetValue(originalPrefabSet));
                    
                    var modifiedValue =  variable.GetValue(prefabSetOverride);
                    if (modifiedValue != null) 
                        variable.SetValue(newPrefabSet, modifiedValue);
                }
                
                PrefabSets.Add(prefabSetMarker.Type, newPrefabSet);
            }

            // Set initialised to avoid skipping 
            __instance.IsInitialised = true;
            // Setup the builder with the newly cached prefab set
            __instance.Builder = new LayoutBuilder(view_data.Floorplan, PrefabSets[prefabSetMarker.Type], __instance.transform);

            // Generate the map
            LayoutMapGenerator.GenerateFor(view_data.Floorplan, true);
            __instance.Builder.Build();
            __instance.UpdateNavmesh();

            // Skip original method as we've done what we needed.
            return false;
        }
    }
}