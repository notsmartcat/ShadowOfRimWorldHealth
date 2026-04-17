namespace ShadowOfRimWorldHealth;

public class RWHealingDifficulty
{
    public RWHealingDifficulty() {}

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
}
public class RWHDBurnBase : RWHealingDifficulty
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
public class RWHDBurn : RWHDBurnBase
{
    public RWHDBurn() : base()
    {
        name = "Burn";

        scar = "Burn scar";

        destroyed = "Burned off";
        destroyedOut = "Burned out";

        description = "A burn.";
    }
}
public class RWHDElectricalBurn : RWHDBurnBase
{
    public RWHDElectricalBurn() : base()
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
public class RWHDCrush : RWHealingDifficulty
{
    public RWHDCrush() : base()
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
public class RWHDCrack : RWHealingDifficulty
{
    public RWHDCrack() : base()
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
public class RWHDCut : RWHealingDifficulty
{
    public RWHDCut() : base()
    {
        name = "Cut";

        infectionChance = 0.15f;

        scar = "Cut scar";

        destroyed = "Cut off";
        destroyedOut = "Cut out";

        description = "A cut.";
    }
}
public class RWHDSurgicalCut : RWHealingDifficulty
{
    public RWHDSurgicalCut() : base()
    {
        name = "Surgical cut";

        infectionChance = 0.15f;

        scar = "Surgical scar";

        destroyed = "Cut off";
        destroyedOut = "Cut out";

        description = "A cut made during surgery.";
    }
}
public class RWHDScratch : RWHealingDifficulty
{
    public RWHDScratch() : base()
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
public class RWHDBite : RWHealingDifficulty
{
    public RWHDBite() : base()
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
public class RWHDStab : RWHealingDifficulty
{
    public RWHDStab() : base()
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
public class RWHDGunshot : RWHealingDifficulty
{
    public RWHDGunshot() : base()
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
public class RWHDShredded : RWHealingDifficulty
{
    public RWHDShredded() : base()
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
public class RWHDBruise : RWHealingDifficulty
{
    public RWHDBruise() : base()
    {
        name = "Bruise";

        bleeding = 0;

        description = "A bruise.";
    }
}
public class RWHDFrostbite : RWHealingDifficulty
{
    public RWHDFrostbite() : base()
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