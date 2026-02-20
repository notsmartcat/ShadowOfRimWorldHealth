using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowOfRimWorldHealth;

public class RWHealingDifficulty
{
    public RWHealingDifficulty() 
    {

    }

    public string name = "Misc";

    public float pain = 1.25f;

    public float scarPain = 0.625f;

    public string oldLabel = "Scar";

    public bool combines = true;

    public int bleeding = 6;

    public int infectionChance = 0;

    public int becomeOldChance = 1;

    public bool naturallyHeals = true;

    public string destroyed = "Destroyed";

    public string destroyedOut = "Destroyed";

    public string treated = "Bandaged";

    public string innerTreated = "Sutured";

    public string solidTreated = "Set";
}

public class RWHDBurn : RWHealingDifficulty
{
    public RWHDBurn() : base()
    {
        name = "Burn";

        pain = 1.875f;

        oldLabel = "Burn scar";

        bleeding = 0;

        infectionChance = 30;

        destroyed = "Burned off";
        destroyedOut = "Burned out";
    }
}
public class RWHDCrush : RWHealingDifficulty
{
    public RWHDCrush() : base()
    {
        name = "Crush";

        bleeding = 1;

        infectionChance = 15;

        destroyed = "Crushed";
        destroyedOut = "Crushed";
    }
}
public class RWHDCrack : RWHealingDifficulty
{
    public RWHDCrack() : base()
    {
        name = "Crack";

        pain = 1;

        bleeding = 0;

        infectionChance = 0;

        becomeOldChance = 0;

        destroyed = "Shattered";
        destroyedOut = "Shattered";

        treated = "Set";
        innerTreated = "Set";
    }
}
public class RWHDCut : RWHealingDifficulty
{
    public RWHDCut() : base()
    {
        name = "Cut";

        oldLabel = "Cut scar";

        infectionChance = 15;

        destroyed = "Cut off";
        destroyedOut = "Cut out";
    }
}
public class RWHDSurgicalCut : RWHealingDifficulty
{
    public RWHDSurgicalCut() : base()
    {
        name = "Surgical cut";

        oldLabel = "Cut scar";

        infectionChance = 15;

        destroyed = "Cut off";
        destroyedOut = "Cut out";
    }
}
public class RWHDScratch : RWHealingDifficulty
{
    public RWHDScratch() : base()
    {
        name = "Scratch";

        oldLabel = "Scratch scar";

        combines = false;

        infectionChance = 15;

        destroyed = "Torn off";
        destroyedOut = "Torn out";
    }
}
public class RWHDBite : RWHealingDifficulty
{
    public RWHDBite() : base()
    {
        name = "Bite";

        oldLabel = "Bite scar";

        combines = false;

        infectionChance = 30;

        destroyed = "Bitten off";
        destroyedOut = "Bitten out";
    }
}
public class RWHDStab : RWHealingDifficulty
{
    public RWHDStab() : base()
    {
        name = "Stab";

        oldLabel = "Stab scar";

        combines = false;

        infectionChance = 15;

        destroyed = "Cut off";
        destroyedOut = "Cut out";
    }
}
public class RWHDGunshot : RWHealingDifficulty
{
    public RWHDGunshot() : base()
    {
        name = "Gunshot";

        oldLabel = "Old gunshot";

        combines = false;

        infectionChance = 15;

        destroyed = "Shot off";
        destroyedOut = "Shot out";
    }
}
public class RWHDShredded : RWHealingDifficulty
{
    public RWHDShredded() : base()
    {
        name = "Shredded";

        infectionChance = 20;

        destroyed = "Torn off";
        destroyedOut = "Torn out";
    }
}
public class RWHDBruise : RWHealingDifficulty
{
    public RWHDBruise() : base()
    {
        name = "Bruise";

        oldLabel = "Old bruise";

        combines = false;

        bleeding = 0;

        infectionChance = 0;

        becomeOldChance = 0;
    }
}
public class RWHDFrostbite : RWHealingDifficulty
{
    public RWHDFrostbite() : base()
    {
        name = "Frostbite";

        oldLabel = "Frostbite scar";

        bleeding = 0;

        infectionChance = 25;

        destroyed = "Lost to frostbite";
    }
}