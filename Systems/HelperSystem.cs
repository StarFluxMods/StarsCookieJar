using Kitchen;
using KitchenMods;
using StarsCookieJar.API;
using StarsCookieJar.Components;
using Unity.Entities;

namespace StarsCookieJar.Systems
{
    public class HelperSystem : GameSystemBase, IModSystem
    {
        public static HelperSystem Instance;

        protected override void Initialise()
        {
            base.Initialise();
            Instance = this;
        }

        public bool DoesEntityHaveComponent(Entity entity)
        {
            if (Has<CSpecialBlueprint>(entity))
            {
                PostHelpers.OpenBlueprintLetter(new EntityContext(EntityManager), entity);
                EntityManager.DestroyEntity(entity);
                return true;
            }

            if (Require(entity, out CApplianceBlueprint cApplianceBlueprint) && CookieJarRegistry.SpecialAppliances.Contains(cApplianceBlueprint.Appliance))
            {
                return true;
            }
            return false;
        }

        protected override void OnUpdate()
        {
        }
    }
}