using System.Collections.Generic;
using System.Linq;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using StarsCookieJar.Components;
using UnityEngine;

namespace StarsCookieJar.API
{
    public class CookieJarRegistry
    {
        internal static Dictionary<int, List<Appliance>> DishLockedDecorations = new Dictionary<int, List<Appliance>>();
        internal static Dictionary<int, List<Decor>> DishLockedDecor = new Dictionary<int, List<Decor>>();
        internal static Dictionary<Appliance, Appliance> ApplianceSpecificLetters = new Dictionary<Appliance, Appliance>();
        internal static List<int> SpecialAppliances = new List<int>();
        internal static Dictionary<FeatureType, LayoutPrefabSet> PrefabOverrides = new Dictionary<FeatureType, LayoutPrefabSet>();
        
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

        public static void RegisterDishLockedDecor(Dish dish, Decor decor)
        {
            if (!DishLockedDecor.ContainsKey(dish.ID))
            {
                DishLockedDecor.Add(dish.ID, new List<Decor>());
            }
            DishLockedDecor[dish.ID].Add(decor);
        }
        
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

            LayoutPrefabSet PrefabSet = ScriptableObject.CreateInstance<LayoutPrefabSet>();
            
            PrefabSet.WallPrefab = WallPrefab;
            PrefabSet.ShortWallPrefab = ShortWallPrefab;
            PrefabSet.HatchPrefab = HatchPrefab;
            PrefabSet.FloorPrefab = FloorPrefab;
            PrefabSet.KitchenFloorPrefab = KitchenFloorPrefab;
            PrefabSet.OutsideFloorPrefab = OutsideFloorPrefab;
            PrefabSet.DoorPrefab = DoorPrefab;
            PrefabSet.DoorPrefabReversed = DoorPrefabReversed;
            PrefabSet.ExternalDoorPrefab = ExternalDoorPrefab;
            PrefabSet.WindowPrefab = WindowPrefab;
            PrefabSet.KitchenWindowPrefab = KitchenWindowPrefab;
            PrefabSet.LegalDoorPrefab = LegalDoorPrefab;
            PrefabSet.OfficeDoorPrefab = OfficeDoorPrefab;
            PrefabSet.TrophyDoorPrefab = TrophyDoorPrefab;
            PrefabSet.EmployeesOnlyDoorPrefab = EmployeesOnlyDoorPrefab;
            PrefabSet.LightDoorPrefab = LightDoorPrefab;
            PrefabSet.MissingDoorPrefab = MissingDoorPrefab;
            PrefabSet.FencePrefab = FencePrefab;
            PrefabSet.DefaultWall = DefaultWall;
            PrefabSet.DefaultFloor = DefaultFloor;
            PrefabSet.KitchenFloor = KitchenFloor;
            PrefabSet.Materials = Materials;

            RegisterCustomPrefabSet(TypeMarker, PrefabSet);
        }
    }
    
}