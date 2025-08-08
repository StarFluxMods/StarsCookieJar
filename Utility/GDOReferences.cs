using KitchenData;
using KitchenLib.Utils;
using StarsCookieJar.Customs.Appliances;

namespace StarsCookieJar.Utility
{
    public class GDOReferences
    {
        public static Appliance DecorativeLetter => (Appliance)GDOUtils.GetCustomGameDataObject<DecorativeLetter>().GameDataObject;
    }
}