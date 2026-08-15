using System.Linq;
using Kitchen;
using KitchenData;
using KitchenMods;
using StarsCookieJar.API;
using Unity.Collections;
using Unity.Entities;

namespace StarsCookieJar.Systems
{
    /*
     * This class is designed to spawn specialty `Decor` on decoration day, based on active dishes
     */
    public class SpawnDishLockedDecor : StartOfDaySystem, IModSystem
    {
        private EntityQuery _CurrentMenuItems;

        protected override void Initialise()
        {
            base.Initialise();
            _CurrentMenuItems = base.GetEntityQuery(typeof(CMenuItem), typeof(CAvailableIngredient));
        }

        protected override void OnUpdate()
        {
            foreach (Decor decor in CookieJarRegistry.DishLockedDecor.Keys.SelectMany(id => CookieJarRegistry.DishLockedDecor[id]))
            {
                decor.IsAvailable = false;
            }

            using (NativeArray<Entity> CurrentMenuItems = _CurrentMenuItems.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity CurrentMenuItem in CurrentMenuItems)
                {
                    if (!Require(CurrentMenuItem, out CMenuItem cMenuItem)) continue;
                    if (!CookieJarRegistry.DishLockedDecor.ContainsKey(cMenuItem.SourceDish)) continue;
                    foreach (Decor decor in CookieJarRegistry.DishLockedDecor[cMenuItem.SourceDish])
                    {
                        decor.IsAvailable = true;
                    }
                }
            }
        }
    }
}