using System.Collections.Generic;
using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using KitchenMods;
using StarsCookieJar.API;
using StarsCookieJar.Components;
using StarsCookieJar.Utility;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace StarsCookieJar.Systems
{
    [UpdateInGroup(typeof(EndOfDayProgressionGroup))]
    [UpdateAfter(typeof(CreateShopRequests))]
    [UpdateAfter(typeof(HandleNewShop))]
    public class SpawnDishLockedDecorations : NightSystem, IModSystem
    {
        private EntityQuery _Blockers;
        private EntityQuery _CurrentMenuItems;
        private EntityQuery _SpawnedMarker;
        private EntityQuery _ExistingLetters;

        private int SpawnCount = 5;
        
        protected override void Initialise()
        {
            base.Initialise();
            _Blockers = GetEntityQuery(new QueryHelper().Any(typeof(CProgressionOption), typeof(CProgressionRequest), typeof(CPopup)));
            _CurrentMenuItems = GetEntityQuery(typeof(CMenuItem), typeof(CAvailableIngredient));
            _SpawnedMarker = GetEntityQuery(typeof(SSpawnedBlueprints));
            _ExistingLetters = GetEntityQuery(typeof(CCreateAppliance), typeof(CPosition));
        }

        protected override void OnUpdate()
        {
            if (!_Blockers.IsEmpty)
            {
                return;
            }

            if (!_SpawnedMarker.IsEmpty)
            {
                return;
            }
            
            int day = GetSingleton<SDay>().Day;

            if (!(day > 0 && day % 5 == 0)) // Return if not decoration day
            {
                return;
            }

            List<Appliance> PotentialDecorations = new List<Appliance>();
            
            using (NativeArray<Entity> CurrentMenuItems = _CurrentMenuItems.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity CurrentMenuItem in CurrentMenuItems)
                {
                    if (!Require(CurrentMenuItem, out CMenuItem cMenuItem)) continue;
                    if (!CookieJarRegistry.DishLockedDecorations.ContainsKey(cMenuItem.SourceDish)) continue;
                    foreach (Appliance appliance in CookieJarRegistry.DishLockedDecorations[cMenuItem.SourceDish])
                    {
                        if (PotentialDecorations.Contains(appliance)) continue;
                        PotentialDecorations.Add(appliance);
                    }
                }
            }

            if (PotentialDecorations.Count == 0)
            {
                return;
            }

            int tile = 0;
            List<Vector3> postTiles = GetPostTiles();
            for (int i = 0; i < SpawnCount; i++)
            {
                PotentialDecorations.ShuffleInPlace();
                Appliance appliance = PotentialDecorations[0];
                Vector3 spawnLocation = FindTile(ref tile, postTiles);
                if (CookieJarRegistry.ApplianceSpecificLetters.TryGetValue(appliance, out Appliance letter))
                {
                    CreateBlueprintLetter(EntityManager, spawnLocation, appliance.ID, letter.ID);
                }
                else
                {
                    CreateBlueprintLetter(EntityManager, spawnLocation, appliance.ID, GDOReferences.DecorativeLetter.ID);
                }
            }
            
            EntityManager.CreateEntity(typeof(SSpawnedBlueprints), typeof(CDestroyApplianceAtDay));
        }
        
        public Vector3 FindTile(ref int placedTile, List<Vector3> floorTiles)
        {
            Vector3 candidate = default(Vector3);
            bool foundValidTile = false;
            while (!foundValidTile && placedTile < floorTiles.Count)
            {
                int counter = placedTile;
                placedTile = counter + 1;
                candidate = floorTiles[counter];
                bool alreadyContainsLetter = false;
                foreach (CPosition cPosition in _ExistingLetters.ToComponentDataArray<CPosition>(Allocator.Temp))
                {
                    if (candidate != cPosition) continue;
                    alreadyContainsLetter = true;
                    break;
                }

                if (alreadyContainsLetter) continue;
                if (TileManager.GetOccupant(candidate) != default) continue;
                
                foundValidTile = true;
                foreach (LayoutPosition layoutPosition in LayoutHelpers.Directions)
                {
                    Entity occupant = TileManager.GetOccupant(candidate + new Vector3(layoutPosition.x, 0, layoutPosition.y));
                    if (occupant == default || !Has<CApplianceTable>(occupant)) continue;
                    foundValidTile = false;
                    break;
                }
            }
            return !foundValidTile ? GetFallbackTile() : candidate;
        }
        
        public static Entity CreateBlueprintLetter(EntityManager entityManager, Vector3 position, int appliance_id, int letter_id)
        {
            Entity letter = entityManager.CreateEntity();
            entityManager.AddComponentData(letter, new CCreateAppliance
            {
                ID = letter_id
            });
            entityManager.AddComponentData(letter, new CPosition(position));
            entityManager.AddComponentData(letter, default(CLetter));
            int price = 0;
            Appliance appliance;
            if (GameData.Main.TryGet(appliance_id, out appliance, true))
            {
                price = appliance.PurchaseCost;
            }
            entityManager.AddComponentData(letter, new CLetterBlueprint
            {
                BlueprintID = AssetReference.Blueprint,
                ApplianceID = appliance_id,
                Price = Mathf.CeilToInt(price)
            });
            entityManager.AddComponentData(letter, default(CShopEntity));
            return letter;
        }
    }
}