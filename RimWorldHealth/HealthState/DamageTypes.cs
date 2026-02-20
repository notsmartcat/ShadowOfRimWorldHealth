using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RWDamageType
{
    public RWDamageType() 
    {

    }

    public string name;

    public string category;

    public string armourCategory;

    public int overkillMin;
    public int overkillMax;

    public List<string> headiffs = new();
}

public class RWCut : RWDamageType
{
    public RWCut() : base()
    {
        name = "Cut";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 10;

        headiffs.Add("Cut");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RWCrush : RWDamageType
{
    public RWCrush() : base()
    {
        name = "Crush";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 40;
        overkillMax = 100;

        headiffs.Add("Crush");
        headiffs.Add("Cut");
        headiffs.Add("Crack");
    }
}
public class RWBlunt : RWDamageType
{
    public RWBlunt() : base()
    {
        name = "Blunt";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 40;
        overkillMax = 100;

        headiffs.Add("Crush");
        headiffs.Add("Bruise");
        headiffs.Add("Crack");
    }
}
public class RWPoke : RWDamageType
{
    public RWPoke() : base()
    {
        name = "Poke";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 40;
        overkillMax = 100;

        headiffs.Add("Crush");
        headiffs.Add("Bruise");
        headiffs.Add("Crack");
    }
}
public class RWDemolish : RWDamageType
{
    public RWDemolish() : base()
    {
        name = "Demolish";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 40;
        overkillMax = 100;

        headiffs.Add("Crush");
        headiffs.Add("Bruise");
        headiffs.Add("Crack");
    }
}

public class RWStab : RWDamageType
{
    public RWStab() : base()
    {
        name = "Stab";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 40;
        overkillMax = 100;

        headiffs.Add("Stab");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}
public class RWRangedStab : RWDamageType
{
    public RWRangedStab() : base()
    {
        name = "Stab";

        category = "Ranged";

        armourCategory = "Sharp";

        overkillMin = 40;
        overkillMax = 100;

        headiffs.Add("Stab");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RWBullet : RWDamageType
{
    public RWBullet() : base()
    {
        name = "Bullet";

        category = "Ranged";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 70;

        headiffs.Add("Gunshot");
    }
}

public class RWScratch : RWDamageType
{
    public RWScratch() : base()
    {
        name = "Scratch";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 70;

        headiffs.Add("Scratch");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}
public class RWBite : RWDamageType
{
    public RWBite() : base()
    {
        name = "Bite";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 10;

        headiffs.Add("Bite");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RWBomb : RWDamageType
{
    public RWBomb() : base()
    {
        name = "Bomb";

        category = "Misc";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 70;

        headiffs.Add("Shredded");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}
public class RWSuperBomb : RWDamageType
{
    public RWSuperBomb() : base()
    {
        name = "Bomb";

        category = "Misc";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Shredded");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RWFlame : RWDamageType
{
    public RWFlame() : base()
    {
        name = "Flame";

        category = "Environmental";

        armourCategory = "Heat";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Burn");
    }
}
public class RWBurn : RWDamageType
{
    public RWBurn() : base()
    {
        name = "Burn";

        category = "Environmental";

        armourCategory = "Heat";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Burn");
    }
}
public class RWFrostbite : RWDamageType
{
    public RWFrostbite() : base()
    {
        name = "Frostbite";

        category = "Environmental";

        armourCategory = "";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Frostbite");
    }
}

public class RWSurgicalCut : RWDamageType
{
    public RWSurgicalCut() : base()
    {
        name = "Surgical cut";

        category = "Medical";

        armourCategory = "";

        overkillMin = 0;
        overkillMax = 10;

        headiffs.Add("Surgical cut");
    }
}