using System.Collections.Generic;
using UnityEngine;

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
                new Head(this),
                new Skull(this),
                new Brain(this),
                new Eye(this)
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
                    bodyPart.name = "Left " + bodyPart.name;
                    this.bodyParts.Add(bodyPart);
                }

                bodyParts[i].name = "Right " + bodyParts[i].name;
                this.bodyParts.Add(bodyParts[i]);
                continue;
            }

            this.bodyParts.Add(bodyParts[i]);
        }
    }

    public void Update()
    {

    }

    public void Damage(string damageType, float damage, RWBodyPart bodyPart)
    {
        bool trye = true;

        RWBodyPart focusedBodyPart = bodyPart;

        focusedBodyPart.health -= damage;

        float extraDamage = 0f;

        Debug.Log(focusedBodyPart.name + " was hit for " + damage + " now it's health is " + focusedBodyPart.health);

        if (focusedBodyPart.health < 0f)
        {
            extraDamage = focusedBodyPart.health * -1;
        }

        while (trye)
        {
            if (focusedBodyPart.isInternal && focusedBodyPart.subPartOf != "")
            {
                for (int i = 0; i < bodyParts.Count; i++)
                {
                    if (bodyParts[i].name == focusedBodyPart.subPartOf)
                    {
                        focusedBodyPart = bodyParts[i];

                        focusedBodyPart.health -= (damage + extraDamage);

                        Debug.Log(focusedBodyPart.name + " was hit for " + damage + " damage and " + extraDamage + " extraDamage now it's health is " + focusedBodyPart.health);

                        extraDamage = 0f;

                        if (focusedBodyPart.health < 0f)
                        {
                            extraDamage = focusedBodyPart.health * -1;
                        }

                        break;
                    }
                }
            }
            else
            {
                trye = false;
            }
        }
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