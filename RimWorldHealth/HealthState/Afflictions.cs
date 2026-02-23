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
                "Shredded" => new RWHDShredded(),
                "Crack" => new RWHDCrack(),
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
public class RWScar : RWInjury
{
    public RWScar(RWPlayerHealthState state, RWBodyPart part, float damage, RWDamageType damageType, string attackerName) : base(state, part, damage, damageType, attackerName)
    {
    }

    public bool isRevealed = false;

    public bool isPermanent = false;

    public float scarDamage = 0;

    public string scarType = "";
}
public class RWDestroyed : RWInjury
{
    public RWDestroyed(RWPlayerHealthState state, RWBodyPart part, float damage, RWDamageType damageType, string attackerName) : base(state, part, damage, damageType, attackerName)
    {
        if (isBleeding && part is not RWOrgan && part is not RWBone)
        {
            healingDifficulty.bleeding = 12;
        }
        else
        {
            isTended = true;
            isBleeding = false;
        }
    }
}
public class RWDisease : RWAffliction
{
    public RWDisease(RWPlayerHealthState state, RWBodyPart part) : base(state, part)
    {

    }

    public string name = "";

    public float severity;
    public float severityGain;

    public float immunity;
    public float immunityGain;

    public float totalTendQuality;
}