using System.Collections.Generic;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using UnityEngine;

namespace StarsCookieJar.Customs.Appliances
{
    public class DecorativeLetter : CustomAppliance
    {
        public override string UniqueNameID => "DecorativeLetter";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("DecorativeLetter").AssignMaterialsByNames();

        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>
        {
            new CFixedRotation(),
            new CImmovable()
        };

        public override void OnRegister(Appliance gameDataObject)
        {
            base.OnRegister(gameDataObject);
            LetterView letterView = gameDataObject.Prefab.AddComponent<LetterView>();
            letterView.Letter = gameDataObject.Prefab.GetChild("Letter");
            letterView.MinDelay = 0;
            letterView.MaxDelay = 2;
            letterView.Animator = gameDataObject.Prefab.GetComponent<Animator>();
        }
    }
}