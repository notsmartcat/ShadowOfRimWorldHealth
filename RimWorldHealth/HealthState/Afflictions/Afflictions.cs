using System;

namespace ShadowOfRimWorldHealth;

public class RWAffliction
{
    public RWAffliction(RWPlayerHealthState state,  RWBodyPart part)
    {
        this.state = state;

        this.part = part;
    }

    public RWPlayerHealthState state;

    public RWBodyPart part;

    public float pain;

    public bool isTended = false;

    public float tendQuality;
}

public class RWInjury : RWAffliction
{
    public RWInjury(RWPlayerHealthState state, RWBodyPart part, float damage, RWDamageType damageType, string attackerName) : base(state, part)
    {
        this.damage = damage;

        this.damageType = damageType;

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
                "Crush" => new RWHDCrush(),
                "Crack" => new RWHDCrack(),
                "Surgical cut" => new RWHDSurgicalCut(),
                "Scratch" => new RWHDScratch(),
                "Bite" => new RWHDBite(),
                "Stab" => new RWHDStab(),
                "Gunshot" => new RWHDGunshot(),
                "Shredded" => new RWHDShredded(),
                "Bruise" => new RWHDBruise(),
                "Frostbite" => new RWHDFrostbite(),
                _ => new RWHealingDifficulty(),
            };

            return healingDifficulty;
        }
    }

    public string attackerName;

    public float damage;

    public RWDamageType damageType;

    public bool isBleeding = true;

    public RWHealingDifficulty healingDifficulty;

    public float infectionTimer;
}

public class RWDisease : RWAffliction
{
    public RWDisease(RWPlayerHealthState state, RWBodyPart part) : base(state, part)
    {
        InfectionLuck = UnityEngine.Random.Range(0.8f, 1.2f);
    }

    public string name = "";

    //Lethal diseases will kill when their severity reaches 100%
    public bool lethal = false;

    //severity ranges from 0 to 1, if the disease is lethal and severity reaches 1 the creature will die
    public float severity = 0;
    //severity gain per cycle
    public float severityGain = 0;
    //severity loss per cycle when the patient is immune
    public float severityLoss = 0;

    public bool isImmune = false;
    //immunity ranges from 0 to 1, if the disease is lethal and immunity reaches 1 before severity the creature will not die
    public float immunity = 0;
    //immunity gain per cycle
    public float immunityGain = 0;

    //treatment will negate severity gain per cycle, the shown treatment amount shows what it would be at 100% tend quality
    public float treatment = 0;

    public float timeUntilTreatment = 0;
    //tend times determine how often a new tend is allowd, this is multiplied off of the cycle time. so if tendTime is 1 and the cycleLength is 13 it will take 13 minutes before a new tend can be done
    public float treatmentTimes = 1;

    //some diseases require the total tend quality to reach 300% before being treated
    public float totalTendQuality = 0;

    public float InfectionLuck = 0;
}