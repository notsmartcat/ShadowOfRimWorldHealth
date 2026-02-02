using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowOfRimWorldHealth;

public class RWInjury
{
    public RWInjury(RWPlayerHealthState state,  RWBodyPart part, float damage, string damageType, string attackerName)
    {
        this.state = state;

        this.part = part;

        this.damage = damage;

        this.damageType = damageType;

        this.attackerName = attackerName;

        if (damage == 99f)
        {
            healingDifficulty = new RWHealingDifficulty(true);
            return;
        }

        healingDifficulty = new RWHealingDifficulty();
    }

    public RWPlayerHealthState state;

    public RWBodyPart part;

    public RWHealingDifficulty healingDifficulty;

    public float damage;

    public string damageType;

    public string attackerName;

    public string scarType = "";

    public bool isBleeding;

    public bool isTended;

    public float tendQuality;

    public float infectionTimer;
}
