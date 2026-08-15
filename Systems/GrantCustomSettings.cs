using Kitchen;
using KitchenData;
using KitchenMods;
using StarsCookieJar.API;
using Unity.Collections;
using Unity.Entities;

namespace StarsCookieJar.Systems
{
    /*
     * This system is designed to grant the player access to a custom `RestaurantSetting`
     * These settings must be registered with `CookieJarRegistry.RegisterCustomSetting()`
     */
    public class GrantCustomSettings : FranchiseFirstFrameSystem, IModSystem
    {
        private EntityQuery _ExistingSettings;
        protected override void Initialise()
        {
            base.Initialise();
            _ExistingSettings = GetEntityQuery(typeof(CSettingUpgrade));
        }

        protected override void OnUpdate()
        {
            using (NativeArray<CSettingUpgrade> ExistingSettings = _ExistingSettings.ToComponentDataArray<CSettingUpgrade>(Allocator.Temp))
            {
                foreach (RestaurantSetting settingToAdd in CookieJarRegistry._standaloneSettings)
                {
                    bool foundDuplicate = false;
                    foreach (CSettingUpgrade cSettingUpgrade in ExistingSettings)
                    {
                        if (cSettingUpgrade.SettingID == settingToAdd.ID)
                        {
                            foundDuplicate = true;
                            break;
                        }
                    }

                    if (foundDuplicate) continue;
                    
                    Entity newEntity = EntityManager.CreateEntity(typeof(CSettingUpgrade));
                    Set(newEntity, new CSettingUpgrade
                    {
                        SettingID = settingToAdd.ID
                    });
                }
            }
        }
    }
}