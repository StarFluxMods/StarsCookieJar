using System.Collections.Generic;
using System.Linq;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using StarsCookieJar.Components;
using UnityEngine;

namespace StarsCookieJar.API
{
    /*
     * The centeral registration area for this mod's functionality
     */
    public class CookieJarRegistry
    {
        #region Dish Locked Decorations
        
        internal static Dictionary<int, List<Appliance>> DishLockedDecorations = new Dictionary<int, List<Appliance>>();
        internal static List<int> SpecialAppliances = new List<int>();

        public static void RegisterDishLockedDecorations(Dish dish, Appliance appliance)
        {
            if (!DishLockedDecorations.ContainsKey(dish.ID))
            {
                DishLockedDecorations.Add(dish.ID, new List<Appliance>());
            }
            DishLockedDecorations[dish.ID].Add(appliance);
            if (!SpecialAppliances.Contains(appliance.ID))
            {
                SpecialAppliances.Add(appliance.ID);
            }
        }
        
        #endregion

        #region Dish Locked Decor

        internal static Dictionary<int, List<Decor>> DishLockedDecor = new Dictionary<int, List<Decor>>();

        public static void RegisterDishLockedDecor(Dish dish, Decor decor)
        {
            if (!DishLockedDecor.ContainsKey(dish.ID))
            {
                DishLockedDecor.Add(dish.ID, new List<Decor>());
            }
            DishLockedDecor[dish.ID].Add(decor);
        }
        
        #endregion

        #region Appliance Specific Letters

        internal static Dictionary<Appliance, Appliance> ApplianceSpecificLetters = new Dictionary<Appliance, Appliance>();

        public static void RegisterApplianceSpecificLetters(Appliance appliance, Appliance letter)
        {
            if (!ApplianceSpecificLetters.ContainsKey(appliance))
            {
                bool foundComponent = letter.Properties.Any(property => property is CSpecialBlueprint);
                if (!foundComponent)
                {
                    letter.Properties.Add(new CSpecialBlueprint());
                }

                ApplianceSpecificLetters.Add(appliance, letter);
            }
            else
            {
                Mod.Logger.LogWarning($"Failed to register letter {letter.name} for {appliance.name}, a letter is already set!");
            }
        }
        
        #endregion

        #region LayoutPrefabSet Overrides
        
        internal static Dictionary<FeatureType, LayoutPrefabSet> PrefabOverrides = new Dictionary<FeatureType, LayoutPrefabSet>();
        
        public static void RegisterCustomPrefabSet(FeatureType TypeMarker, LayoutPrefabSet PrefabSet)
        {
            if (!PrefabOverrides.ContainsKey(TypeMarker))
            {
                PrefabOverrides.Add(TypeMarker, PrefabSet);
            }
            else
            {
                Mod.Logger.LogWarning($"Failed to register FeatureType Override {TypeMarker}, an override is already set!");
            }
        }
        
        public static void RegisterCustomPrefabSet(FeatureType TypeMarker
            ,GameObject WallPrefab = null
            ,GameObject ShortWallPrefab = null
            ,GameObject HatchPrefab = null
            ,GameObject FloorPrefab = null
            ,GameObject KitchenFloorPrefab = null
            ,GameObject OutsideFloorPrefab = null
            ,GameObject DoorPrefab = null
            ,GameObject DoorPrefabReversed = null
            ,GameObject ExternalDoorPrefab = null
            ,GameObject WindowPrefab = null
            ,GameObject KitchenWindowPrefab = null
            ,GameObject LegalDoorPrefab = null
            ,GameObject OfficeDoorPrefab = null
            ,GameObject TrophyDoorPrefab = null
            ,GameObject EmployeesOnlyDoorPrefab = null
            ,GameObject LightDoorPrefab = null
            ,GameObject MissingDoorPrefab = null
            ,GameObject FencePrefab = null
            ,Material DefaultWall = null
            ,Material DefaultFloor = null
            ,Material KitchenFloor = null
            ,Dictionary<LayoutPrefabSet.MaterialType, Material> Materials = null
            )
        {

            if (PrefabOverrides.ContainsKey(TypeMarker))
            {
                Mod.Logger.LogWarning($"Failed to register FeatureType Override {TypeMarker}, an override is already set!");
                return;
            }

            LayoutPrefabSet prefabSet = ScriptableObject.CreateInstance<LayoutPrefabSet>();
            
            prefabSet.WallPrefab = WallPrefab;
            prefabSet.ShortWallPrefab = ShortWallPrefab;
            prefabSet.HatchPrefab = HatchPrefab;
            prefabSet.FloorPrefab = FloorPrefab;
            prefabSet.KitchenFloorPrefab = KitchenFloorPrefab;
            prefabSet.OutsideFloorPrefab = OutsideFloorPrefab;
            prefabSet.DoorPrefab = DoorPrefab;
            prefabSet.DoorPrefabReversed = DoorPrefabReversed;
            prefabSet.ExternalDoorPrefab = ExternalDoorPrefab;
            prefabSet.WindowPrefab = WindowPrefab;
            prefabSet.KitchenWindowPrefab = KitchenWindowPrefab;
            prefabSet.LegalDoorPrefab = LegalDoorPrefab;
            prefabSet.OfficeDoorPrefab = OfficeDoorPrefab;
            prefabSet.TrophyDoorPrefab = TrophyDoorPrefab;
            prefabSet.EmployeesOnlyDoorPrefab = EmployeesOnlyDoorPrefab;
            prefabSet.LightDoorPrefab = LightDoorPrefab;
            prefabSet.MissingDoorPrefab = MissingDoorPrefab;
            prefabSet.FencePrefab = FencePrefab;
            prefabSet.DefaultWall = DefaultWall;
            prefabSet.DefaultFloor = DefaultFloor;
            prefabSet.KitchenFloor = KitchenFloor;
            prefabSet.Materials = Materials;

            RegisterCustomPrefabSet(TypeMarker, prefabSet);
        }
        
        #endregion

        #region Custom Settings / Layouts

        internal static List<RestaurantSetting> _standaloneSettings = new List<RestaurantSetting>();
        internal static Dictionary<int, List<int>> _settingLayoutPairs =  new Dictionary<int, List<int>>();
        
        public static bool RegisterCustomSetting(RestaurantSetting setting)
        {
            if (_standaloneSettings.Contains(setting)) return false;
            _standaloneSettings.Add(setting);
            return true;
        }

        public static bool RegisterLayoutToSetting(RestaurantSetting setting, LayoutProfile layout, bool addForcedLayoutIntoPool = false)
        {
            if (!_settingLayoutPairs.ContainsKey(setting.ID))
            {
                _settingLayoutPairs.Add(setting.ID, new List<int>());
            }
            
            if (_settingLayoutPairs[setting.ID].Contains(layout.ID))  return false;
            _settingLayoutPairs[setting.ID].Add(layout.ID);

            if (addForcedLayoutIntoPool && setting.ForceLayout != null)
            {
                _settingLayoutPairs[setting.ID].Add(setting.ForceLayout.ID);
                setting.ForceLayout = null;
            }

            return true;
        }

        #endregion
        
    }
    
}