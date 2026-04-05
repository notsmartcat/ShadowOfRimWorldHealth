namespace ShadowOfRimWorldHealth;

public class RWScar : RWInjury
{
    public RWScar(CreatureState state, RWBodyPart part, float damage, RWDamageType damageType, string attackName = "", string attackerName = "") : base(state, part, damage, damageType, attackName, attackerName)
    {
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