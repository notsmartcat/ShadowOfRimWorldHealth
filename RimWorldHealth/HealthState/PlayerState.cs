using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RWHealthState : CreatureState
{
    public RWHealthState(AbstractCreature crit) : base(crit)
    {

    }

    public List<RWBodyPart> bodyParts = new();

    public float blood;

    public float pain;

    public float consciousness;

    public float moving;

    public float manipulation;

    public float talking;

    public float eating;

    public float sight;

    public float hearing;

    public float breathing;

    public float bloodFiltration;

    public float bloodPumping;

    public float digestion;
}

public class RWPlayerHealthState : PlayerState
{
    public RWPlayerHealthState(AbstractCreature crit, int playerNumber, SlugcatStats.Name slugcatCharacter, bool isGhost) : base(crit, playerNumber, slugcatCharacter, isGhost)
    {
        List<RWBodyPart> bodyParts = new(3) {
                new UpperTorso(this),
                new LowerTorso(this),
                new Head(this)
            };

        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (bodyParts[i].quantity == 2)
            {
                RWBodyPart bodyPart = null;
                if (bodyParts[i] is Eye)
                {
                    bodyPart = new Eye(this);
                }

                if (bodyPart != null)
                {
                    this.bodyParts.Add(bodyPart);
                }

                this.bodyParts.Add(bodyParts[i]);
                continue;
            }

            this.bodyParts.Add(bodyParts[i]);
        }
    }

    public void Update()
    {

    }

    public List<RWBodyPart> bodyParts = new();

    public float blood = 100;

    public float pain = 0;

    public float consciousness = 100;

    public float moving = 100;

    public float manipulation = 100;

    public float talking = 100;

    public float eating = 100;

    public float sight = 100;

    public float hearing = 100;

    public float breathing = 100;

    public float bloodFiltration = 100;

    public float bloodPumping = 100;

    public float digestion = 100;
}