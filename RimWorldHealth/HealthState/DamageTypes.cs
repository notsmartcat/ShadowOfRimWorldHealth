using System.Collections.Generic;

namespace ShadowOfRimWorldHealth
{
    internal class DamageType
    {
        public DamageType() 
        {

        }

        string name;

        string label;

        string category;

        string armourCategory;

        int overkillMin;
        int overkillMax;

        List<string> headiffs = new();

        bool outerLayers;
    }
}
