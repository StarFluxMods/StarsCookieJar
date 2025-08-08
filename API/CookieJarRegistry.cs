using System.Collections.Generic;
using System.Linq;
using KitchenData;
using StarsCookieJar.Components;

namespace StarsCookieJar.API
{
    public class CookieJarRegistry
    {
        internal static Dictionary<int, List<Appliance>> DishLockedDecorations = new Dictionary<int, List<Appliance>>();
        internal static Dictionary<int, List<Decor>> DishLockedDecor = new Dictionary<int, List<Decor>>();
        internal static Dictionary<Appliance, Appliance> ApplianceSpecificLetters = new Dictionary<Appliance, Appliance>();
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
    }
    
}