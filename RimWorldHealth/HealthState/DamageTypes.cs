using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RWDamageType
{
    public RWDamageType() 
    {

    }

    public string name;

    public string label;

    public string category;

    public string armourCategory;

    public int overkillMin;
    public int overkillMax;

    public List<string> headiffs = new();
}

public class RWBomb : RWDamageType
{
    public RWBomb() : base()
    {
        name = "Bomb";

        label = "bomb";

        category = "Misc";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 70;

        headiffs.Add("Shredded");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}
