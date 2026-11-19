namespace ShadowOfRimWorldHealth;

public class RimAffliction(CreatureState state, RimBodyPart part)
{
    public void Remove()
    {
        state = null;
        part = null;
    }

    public CreatureState state = state;

    public RimBodyPart part = part;

    public UnityEngine.Color labelColour = new(1f, 1f, 1f);
}

public class RimTendableAffliction(CreatureState state, RimBodyPart part) : RimAffliction(state, part)
{
    public float pain;

    public bool isTended = false;

    public float tendQuality;

    public bool isCharacterSpecific = false;
}

public class RimInjury : RimTendableAffliction
{
    public RimInjury(CreatureState state, RimBodyPart part, float damage, RimDamageType damageType, string attackName = "", string attackerName = "") : base(state, part)
    {
        this.damage = damage;

        this.damageType = damageType;

        this.attackName = attackName;
        this.attackerName = attackerName;

        if (damageType != null && damageType.headiffs.Count > 0 && damageType.headiffs[0] != "")
        {
            healingDifficulty = HealingDifficulty();

            if (attackName == "Coalescipede - Fangs" || attackName == "Locust swarm - Mandibles")
            {
                healingDifficulty.combines = true;
            }
        }
        else
        {
            healingDifficulty = new RimHealingDifficulty();
        }

        if (part.isSolid || part.isBionic || healingDifficulty.bleeding <= 0)
        {
            isBleeding = false;
        }

        if (healingDifficulty.infectionChance > 0)
        {
            infectionTimer = UnityEngine.Random.Range(15000, 45001);
        }

        RimHealingDifficulty HealingDifficulty()
        {
            string RWHealingDifficultyName;

            if (damageType.headiffs.Count > 1 && damageType.headiffs[1] != "" && !part.isSolid && !part.isBionic && !part.isInternal)
            {
                RWHealingDifficultyName = damageType.headiffs[1];
            }
            else if (damageType.headiffs.Count > 2 && damageType.headiffs[2] != "" && (part.isSolid || part.isBionic))
            {
                RWHealingDifficultyName = damageType.headiffs[2];
            }
            else
            {
                RWHealingDifficultyName = damageType.headiffs[0];
            }

            RimHealingDifficulty healingDifficulty = RimHealingDifficulty.GetRWHealingDifficulty(RWHealingDifficultyName);

            return healingDifficulty;
        }
    }

    public RimInjury(CreatureState state, RimBodyPart part) : base(state, part) {}

    public string attackName; //Name of the attack, used in the HealthTab
    public string attackerName; //Name of the attacker (currently unused but will be used in the HealthTab)

    public float damage;
    public RimDamageType damageType;

    public bool isBleeding = true;

    public float infectionTimer; //If the wound is injury bleeds and the Healing Difficulty has infection chance a random time between 4.17 and 12.5 minutes will be set, when the timer finishes the Infection will be rolled

    public RimHealingDifficulty healingDifficulty;

    public void RemoveSelf()
    {
        if (part != null)
        {
            part.afflictions.Remove(this);
            part = null;
        }

        state = null;
        healingDifficulty = null;
    }
}

public class RimDisease : RimTendableAffliction
{
    public RimDisease(CreatureState state, RimBodyPart part) : base(state, part)
    {
        labelColour = new(0.9f, 1f, 0.35f);
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
    public float treatmentTimes = 1; //Tend times determine how often a new tend is allowed, this is multiplied off of the cycle time. so if tendTime is 0.5 and the cycleLength is 13 it will take 6.5 minutes before a new tend can be done

    public float totalTendQuality = 0; //Some diseases require the total tend quality to reach 300% before being treated

    public float InfectionLuck = UnityEngine.Random.Range(0.8f, 1.2f); //Infection luck ranges from 0.8 to 1.2 and it multiplies the severity gained/lost

    public void RemoveSelf()
    {
        if (part != null)
        {
            part.afflictions.Remove(this);
            part = null;
        }

        state = null;
    }
}

public class RimAilment(CreatureState state, RimBodyPart part) : RimAffliction(state, part)
{
    public float amount;
    public float lastAmount;
} //Ailments cannot be treated and serve simply to show information on the HealthTab, these afflictions can affect capacities

public class RimArtificialPartBase : RimAffliction
{
    public RimArtificialPartBase(CreatureState state, RimBodyPart part) : base(state, part)
    {
        labelColour = new(0.6f, 0.6f, 1f);
    }

    public string name = "Default Articicial Part";
    public float efficiency = 0;
    public string descrition = "Default description";
}