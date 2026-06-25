namespace ShadowOfRimWorldHealth;

public class RWScar : RWInjury
{
    public RWScar(CreatureState state, RWBodyPart part, float damage, RWDamageType damageType, string attackName = "", string attackerName = "") : base(state, part, damage, damageType, attackName, attackerName)
    {
    }

    public RWScar(CreatureState state, RWBodyPart part, float tendQuality, string attackName, string attackerName, float damage, RWDamageType damageType, float infectionTimer, RWHealingDifficulty healingDifficulty, bool isRevealed, bool isPermanent, float scarDamage, string painCategory) : base(state, part, tendQuality, attackName, attackerName, damage, damageType, infectionTimer, healingDifficulty)
    {
        this.isRevealed = isRevealed;
        this.isPermanent = isPermanent;
        this.scarDamage = scarDamage;
        this.painCategory = painCategory;
    }

    public bool isRevealed = false; //Scars reveal when the injuries damage get's healed to the scarDamage

    public bool isPermanent = false; //Permanent scars get revealed on creation and they use the total damage dealt rather then a fraction

    public float scarDamage = 0; //Non-permanent scars will act as regular injuries until their damage get's to this threshold, then they will be revealed

    public string painCategory = ""; //The pain category consist of: Painless (no pain), Low (50% of pain), Medium (100% pain), High (150% pain)
}
public class RWDestroyed : RWInjury
{
    public RWDestroyed(CreatureState state, RWBodyPart part, float damage, RWDamageType damageType, string attackName = "", string attackerName = "") : base(state, part, damage, damageType, attackName, attackerName)
    {
        if (damageType is RWFrostbite)
        {
            healingDifficulty.bleeding = 1;
        }

        if (healingDifficulty.bleeding == 0 || damage == 0 || part.isSolid)
        {
            isTended = true;
            isBleeding = false;
        }
        else
        {
            isTended = false;
            isBleeding = true;
        }
    }

    public RWDestroyed(CreatureState state, RWBodyPart part, float tendQuality, string attackName, string attackerName, float damage, RWDamageType damageType, float infectionTimer, RWHealingDifficulty healingDifficulty) : base(state, part, tendQuality, attackName, attackerName, damage, damageType, infectionTimer, healingDifficulty)
    {
        isFresh = false;
    }

    public bool isFresh = true;
}