namespace ShadowOfRimWorldHealth;

public class RimHealingDifficulty
{
    public RimHealingDifficulty() {}

    public string name = "Misc"; //The name that is used in the HealthTab

    public float pain = 1.25f; //The amount of pain per damage of the injury
    public float scarPain = 0.625f; //The amount of pain per damage of the scar

    public int bleeding = 6; //The amount of bleed per damage

    public float infectionChance = 0; //Base chance of this would becoming Infected. Treating the would lowers the chance

    public bool combines = false;

    public string scar = "Scar";
    public string permanentScar = "";

    public string treated = "Bandaged";
    public string innerTreated = "Sutured";
    public string solidTreated = "Set";

    public string destroyed = "Destroyed";
    public string destroyedOut = "Destroyed";

    public string description = "Miscellaneous injuries.";

    public static RimHealingDifficulty GetRWHealingDifficulty(string name)
    {
        RimHealingDifficulty healingDifficulty = name switch
        {
            "Burn" => new RimHDBurn(),
            "Electrical burn" => new RimHDElectricalBurn(),
            "Crush" => new RimHDCrush(),
            "Crack" => new RimHDCrack(),
            "Cut" => new RimHDCut(),
            "Surgical cut" => new RimHDSurgicalCut(),
            "Scratch" => new RimHDScratch(),
            "Bite" => new RimHDBite(),
            "Stab" => new RimHDStab(),
            "Gunshot" => new RimHDGunshot(),
            "Shredded" => new RimHDShredded(),
            "Bruise" => new RimHDBruise(),
            "Frostbite" => new RimHDFrostbite(),
            "Acid burn" => new RimHDAcidBurn(),
            _ => null,
        };

        //Space to add custom HealingDifficulties

        healingDifficulty ??= new();

        return healingDifficulty;
    }
}

public class RimHDCut : RimHealingDifficulty
{
    public RimHDCut() : base()
    {
        name = "Cut";

        infectionChance = 0.15f;

        scar = "Cut scar";

        destroyed = "Cut off";
        destroyedOut = "Cut out";

        description = "A cut.";
    }
}

public class RimHDCrush : RimHealingDifficulty
{
    public RimHDCrush() : base()
    {
        name = "Crush";

        combines = true;

        bleeding = 1;

        infectionChance = 0.15f;

        scar = "Mangled scar";

        destroyed = "Crushed";
        destroyedOut = "Crushed";

        description = "A crushing wound.";
    }
}
public class RimHDShredded : RimHealingDifficulty
{
    public RimHDShredded() : base()
    {
        name = "Shredded";

        infectionChance = 0.2f;

        combines = true;

        scar = "Shredded scar";

        destroyed = "Torn off";
        destroyedOut = "Torn out";

        description = "A part of the body has been shredded and torn.";
    }
}
public class RimHDBruise : RimHealingDifficulty
{
    public RimHDBruise() : base()
    {
        name = "Bruise";

        bleeding = 0;

        description = "A bruise.";
    }
}
public class RimHDCrack : RimHealingDifficulty
{
    public RimHDCrack() : base()
    {
        name = "Crack";

        combines = true;

        pain = 1;

        bleeding = 0;

        scar = "Permanent crack";

        treated = "Set";
        innerTreated = "Set";
        solidTreated = "Set";

        destroyed = "Shattered";
        destroyedOut = "Shattered";

        description = "A crack.";
    }
}

public class RimHDStab : RimHealingDifficulty
{
    public RimHDStab() : base()
    {
        name = "Stab";

        combines = false;

        infectionChance = 0.15f;

        scar = "Stab scar";

        destroyed = "Cut off";
        destroyedOut = "Cut out";

        description = "A stab wound.";
    }
}

public class RimHDGunshot : RimHealingDifficulty
{
    public RimHDGunshot() : base()
    {
        name = "Gunshot";

        infectionChance = 0.15f;

        scar = "Old gunshot";
        permanentScar = "Permanent gunshot injury";

        destroyed = "Shot off";
        destroyedOut = "Shot out";

        description = "A gunshot wound.";
    }
}

public class RimHDScratch : RimHealingDifficulty
{
    public RimHDScratch() : base()
    {
        name = "Scratch";

        combines = false;

        infectionChance = 0.15f;

        scar = "Scratch scar";

        destroyed = "Torn off";
        destroyedOut = "Torn out";

        description = "A scratch or tear.";
    }
}
public class RimHDBite : RimHealingDifficulty
{
    public RimHDBite() : base()
    {
        name = "Bite";

        combines = false;

        infectionChance = 0.3f;

        scar = "Bite scar";

        destroyed = "Bitten off";
        destroyedOut = "Bitten out";

        description = "A bite wound.";
    }
}

public class RWHDBurnBase : RimHealingDifficulty
{
    public RWHDBurnBase() : base()
    {
        name = "Burn";

        pain = 1.875f;

        bleeding = 0;

        infectionChance = 0.3f;

        combines = true;

        innerTreated = "Tended";
        solidTreated = "Tended";

        description = "";
    }
}
public class RimHDBurn : RWHDBurnBase
{
    public RimHDBurn() : base()
    {
        name = "Burn";

        scar = "Burn scar";

        destroyed = "Burned off";
        destroyedOut = "Burned out";

        description = "A burn.";
    }
}
public class RimHDFrostbite : RimHealingDifficulty
{
    public RimHDFrostbite() : base()
    {
        name = "Frostbite";

        bleeding = 0;

        infectionChance = 0.25f;

        combines = true;

        innerTreated = "Tended";
        solidTreated = "Tended";

        scar = "Frostbite scar";

        destroyed = "Lost to frostbite";

        description = "Frozen tissue caused by exposure to cold. Frostbite is very painful, and frostbitten body parts are often lost.";
    }
}
public class RimHDElectricalBurn : RWHDBurnBase
{
    public RimHDElectricalBurn() : base()
    {
        name = "Electrical burn";

        pain = 1.875f;

        bleeding = 0;

        infectionChance = 0.3f;

        scar = "Electrical burn scar";

        destroyed = "Burned off";
        destroyedOut = "Burned out";

        description = "An electrical burn.";
    }
}
public class RimHDAcidBurn : RWHDBurnBase
{
    public RimHDAcidBurn() : base()
    {
        name = "Acid burn";

        scar = "Acid burn scar";

        destroyed = "Dissolved off";
        destroyedOut = "Dissolved out";

        description = "An acid burn.";
    }
}

public class RimHDSurgicalCut : RimHealingDifficulty
{
    public RimHDSurgicalCut() : base()
    {
        name = "Surgical cut";

        infectionChance = 0.15f;

        scar = "Surgical scar";

        destroyed = "Cut off";
        destroyedOut = "Cut out";

        description = "A cut made during surgery.";
    }
}