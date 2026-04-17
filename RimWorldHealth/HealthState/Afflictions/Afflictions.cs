namespace ShadowOfRimWorldHealth;

public class RWAffliction
{
    public RWAffliction(CreatureState state,  RWBodyPart part)
    {
        this.state = state;

        this.part = part;
    }

    public CreatureState state;

    public RWBodyPart part;

    public float pain;

    public bool isTended = false;

    public float tendQuality;
}

public class RWInjury : RWAffliction
{
    public RWInjury(CreatureState state, RWBodyPart part, float damage, RWDamageType damageType, string attackName = "", string attackerName = "") : base(state, part)
    {
        this.damage = damage;

        this.damageType = damageType;

        this.attackName = attackName;
        this.attackerName = attackerName;

        if (damageType != null && damageType.headiffs.Count > 0 && damageType.headiffs[0] != "")
        {
            healingDifficulty = HealingDifficulty();
        }
        else
        {
            healingDifficulty = new RWHealingDifficulty();
        }

        if (part.isSolid || healingDifficulty.bleeding <= 0)
        {
            isBleeding = false;
        }

        if (healingDifficulty.infectionChance > 0)
        {
            infectionTimer = UnityEngine.Random.Range(15000, 45001);
        }

        RWHealingDifficulty HealingDifficulty()
        {
            string RWHealingDifficultyName;

            if (damageType.headiffs.Count > 1 && damageType.headiffs[1] != "" && !part.isSolid && !part.isInternal)
            {
                RWHealingDifficultyName = damageType.headiffs[1];
            }
            else if (damageType.headiffs.Count > 2 && damageType.headiffs[2] != "" && part.isSolid)
            {
                RWHealingDifficultyName = damageType.headiffs[2];
            }
            else
            {
                RWHealingDifficultyName = damageType.headiffs[0];
            }

            RWHealingDifficulty healingDifficulty = RWHealingDifficultyName switch
            {
                "Burn" => new RWHDBurn(),
                "Electrical burn" => new RWHDElectricalBurn(),
                "Crush" => new RWHDCrush(),
                "Crack" => new RWHDCrack(),
                "Cut" => new RWHDCut(),
                "Surgical cut" => new RWHDSurgicalCut(),
                "Scratch" => new RWHDScratch(),
                "Bite" => new RWHDBite(),
                "Stab" => new RWHDStab(),
                "Gunshot" => new RWHDGunshot(),
                "Shredded" => new RWHDShredded(),
                "Bruise" => new RWHDBruise(),
                "Frostbite" => new RWHDFrostbite(),
                _ => null,
            };

            //Space to add custom HealingDifficulties

            healingDifficulty ??= new();

            return healingDifficulty;
        }
    }

    public string attackName; //Name of the attack, used in the HealthTab
    public string attackerName; //Name of the attacker (currently unused but will be used in the HealthTab)

    public float damage;
    public RWDamageType damageType;

    public bool isBleeding = true;

    public float infectionTimer; //If the wound is injury bleeds and the Healing Difficulty has infection chance a random time between 4.17 and 12.5 minutes will be set, when the timer finishes the Infection will be rolled

    public RWHealingDifficulty healingDifficulty;
}

public class RWDisease : RWAffliction
{
    public RWDisease(CreatureState state, RWBodyPart part) : base(state, part)
    {
        InfectionLuck = UnityEngine.Random.Range(0.8f, 1.2f);
    }

    public string name = "";

    public bool lethal = false; //Lethal diseases will kill when their severity reaches 100%

    public float severity = 0; //Severity ranges from 0 to 1, if the disease is lethal and severity reaches 1 the creature will die
    public float severityGain = 0; //Severity gain per cycle
    public float severityLoss = 0; //Severity loss per cycle when the patient is immune

    public bool isImmune = false;
    public float immunity = 0; //Immunity ranges from 0 to 1, if the disease is lethal and immunity reaches 1 before severity the creature will not die
    public float immunityGain = 0; //Immunity gain per cycle

    public float treatment = 0; //Treatment will negate severity gain per cycle, the shown treatment amount shows what it would be at 100% tend quality

    public float timeUntilTreatment = 0;
    public float treatmentTimes = 1; //Tend times determine how often a new tend is allowd, this is multiplied off of the cycle time. so if tendTime is 0.5 and the cycleLength is 13 it will take 6.5 minutes before a new tend can be done

    public float totalTendQuality = 0; //Some diseases require the total tend quality to reach 300% before being treated

    public float InfectionLuck = 0; //Infection luck ranges from 0.8 to 1.2 and it multiplies the severity gained/lost
}

public class RWInformational : RWAffliction
{
    public RWInformational(CreatureState state, RWBodyPart part) : base(state, part)
    {
        isTended = true;
    }
} //Informational afflictions cannot be treated and serve simply to show information on the HealthTab, these affliction can affect capacities