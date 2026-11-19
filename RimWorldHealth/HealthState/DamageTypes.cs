using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RimDamageType
{
    public RimDamageType() {}

    public string name;

    public string category;

    public string armourCategory;

    public float overkillMin;
    public float overkillMax;

    public List<string> headiffs = new();

    public static RimDamageType GetRWDamageType(string name)
    {
        RimDamageType damageType = name switch
        {
            "RWCut" => new RimCut(),
            "RWCrush" => new RimCrush(),
            "RWBlunt" => new RimBlunt(),
            "RWPoke" => new RimPoke(),
            "RWDemolish" => new RimDemolish(),
            "RWStab" => new RimStab(),
            "RWBullet" => new RimBullet(),
            "RWScratch" => new RimScratch(),
            "RWBite" => new RimBite(),
            "RWBomb" => new RimBomb(),
            "RWSuperBomb" => new RimSuperBomb(),
            "RWFlame" => new RimFlame(),
            "RWBurn" => new RimBurn(),
            "RWFrostbite" => new RimFrostbite(),
            "RWElectricalBurn" => new RimElectricalBurn(),
            "RWAcidBurn" => new RimAcidBurn(),
            "RWSurgicalCut" => new RimSurgicalCut(),
            _ => null,
        };

        //Space to add custom DamageTypes

        damageType ??= new RimPoke();

        return damageType;
    }
}

public class RimCut : RimDamageType
{
    public RimCut() : base()
    {
        name = "Cut";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0.1f;

        headiffs.Add("Cut");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RimCrush : RimDamageType
{
    public RimCrush() : base()
    {
        name = "Crush";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 0.4f;
        overkillMax = 1;

        headiffs.Add("Crush");
        headiffs.Add("Cut");
        headiffs.Add("Crack");
    }
}
public class RimBlunt : RimDamageType
{
    public RimBlunt() : base()
    {
        name = "Blunt";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 0.4f;
        overkillMax = 1;

        headiffs.Add("Crush");
        headiffs.Add("Bruise");
        headiffs.Add("Crack");
    }
}
public class RimPoke : RimDamageType
{
    public RimPoke() : base()
    {
        name = "Poke";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 0.4f;
        overkillMax = 1;

        headiffs.Add("Crush");
        headiffs.Add("Bruise");
        headiffs.Add("Crack");
    }
}
public class RimDemolish : RimDamageType
{
    public RimDemolish() : base()
    {
        name = "Demolish";

        category = "Melee";

        armourCategory = "Blunt";

        overkillMin = 0.4f;
        overkillMax = 1;

        headiffs.Add("Crush");
        headiffs.Add("Bruise");
        headiffs.Add("Crack");
    }
}

public class RimStab : RimDamageType
{
    public RimStab() : base()
    {
        name = "Stab";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0.4f;
        overkillMax = 1;

        headiffs.Add("Stab");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RimBullet : RimDamageType
{
    public RimBullet() : base()
    {
        name = "Bullet";

        category = "Ranged";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0.7f;

        headiffs.Add("Gunshot");
    }
}

public class RimScratch : RimDamageType
{
    public RimScratch() : base()
    {
        name = "Scratch";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0.7f;

        headiffs.Add("Scratch");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}
public class RimBite : RimDamageType
{
    public RimBite() : base()
    {
        name = "Bite";

        category = "Melee";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0.1f;

        headiffs.Add("Bite");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}

public class RimBomb : RimDamageType
{
    public RimBomb() : base()
    {
        name = "Bomb";

        category = "Misc";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0.7f;

        headiffs.Add("Shredded");
        headiffs.Add("");
        headiffs.Add("Crack");
    }
}
public class RimSuperBomb : RimDamageType
{
    public RimSuperBomb() : base()
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

public class RimFlame : RimDamageType
{
    public RimFlame() : base()
    {
        name = "Flame";

        category = "Environmental";

        armourCategory = "Heat";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Burn");
    }
}
public class RimBurn : RimDamageType
{
    public RimBurn() : base()
    {
        name = "Burn";

        category = "Environmental";

        armourCategory = "Heat";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Burn");
    }
}
public class RimFrostbite : RimDamageType
{
    public RimFrostbite() : base()
    {
        name = "Frostbite";

        category = "Environmental";

        armourCategory = "";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Frostbite");
    }
}
public class RimElectricalBurn : RimDamageType
{
    public RimElectricalBurn() : base()
    {
        name = "Electrical burn";

        category = "Environmental";

        armourCategory = "Heat";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Electrical burn");
    }
}
public class RimAcidBurn : RimDamageType
{
    public RimAcidBurn() : base()
    {
        name = "Acid burn";

        category = "Misc";

        armourCategory = "Sharp";

        overkillMin = 0;
        overkillMax = 0;

        headiffs.Add("Acid burn");
    }
}

public class RimSurgicalCut : RimDamageType
{
    public RimSurgicalCut() : base()
    {
        name = "Surgical cut";

        category = "Medical";

        armourCategory = "";

        overkillMin = 0;
        overkillMax = 0.1f;

        headiffs.Add("Surgical cut");
    }
}