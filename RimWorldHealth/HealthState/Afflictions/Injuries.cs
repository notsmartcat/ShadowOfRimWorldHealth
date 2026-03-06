namespace ShadowOfRimWorldHealth;

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