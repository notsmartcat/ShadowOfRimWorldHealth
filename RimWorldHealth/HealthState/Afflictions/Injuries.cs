namespace ShadowOfRimWorldHealth;

public class RimScar : RimInjury
{
    public RimScar(CreatureState state, RimBodyPart part, float damage, RimDamageType damageType, string attackName = "", string attackerName = "") : base(state, part, damage, damageType, attackName, attackerName) {}

    public RimScar(CreatureState state, RimBodyPart part) : base(state, part) {}

    public bool isRevealed = false; //Scars reveal when the injuries damage get's healed to the scarDamage

    public bool isPermanent = false; //Permanent scars get revealed on creation and they use the total damage dealt rather then a fraction

    public float scarDamage = 0; //Non-permanent scars will act as regular injuries until their damage get's to this threshold, then they will be revealed

    public string painCategory = ""; //The pain category consist of: Painless (no pain), Low (50% of pain), Medium (100% pain), High (150% pain)
}

public class RimDestroyed : RimInjury
{
    public RimDestroyed(CreatureState state, RimBodyPart part, float damage, RimDamageType damageType, string attackName = "", string attackerName = "") : base(state, part, damage, damageType, attackName, attackerName)
    {
        if (damageType is RimFrostbite)
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

        labelColour = labelColour = new(0.5f, 0.5f, 0.5f);
    }

    public RimDestroyed(CreatureState state, RimBodyPart part) : base(state, part)
    {
        isFresh = false;

        labelColour = labelColour = new(0.5f, 0.5f, 0.5f);
    }

    public bool isFresh = true;
}