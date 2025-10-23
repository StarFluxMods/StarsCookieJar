using Kitchen.Layouts;
using Kitchen.Layouts.Features;
using Kitchen.Layouts.Modules;

namespace StarsCookieJar.Customs.LayoutProfile.Nodes
{
    public class TypeMarker : LayoutModule
    {
        public FeatureType MarkerType;
        public override void ActOn(LayoutBlueprint blueprint)
        {
            blueprint.Features.Add(new Feature(default, default, MarkerType));
        }
    }
}