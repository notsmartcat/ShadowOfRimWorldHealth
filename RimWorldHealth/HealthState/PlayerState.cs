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
        bodySizeFactor = 1;

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

        for (int i = 0; i < bodyParts.Count; i++)
        {
            maxHealth += bodyParts[i].maxHealth;
        }
    }

    public void Update()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            float health = bodyParts[i].health;

            for (int j = 0; j < bodyParts[i].injuries.Count; j++)
            {

            }
        }
    }

    public void Damage(string damageType, float damage, RWBodyPart bodyPart)
    {
        bool trye = true;

        RWBodyPart focusedBodyPart = bodyPart;

        float health = focusedBodyPart.health;
        health -= damage;

        float extraDamage = 0f;

        Debug.Log(focusedBodyPart.name + " was hit for " + damage);

        if (focusedBodyPart.health < 0f)
        {
            DestroyBodyPart();
            extraDamage = focusedBodyPart.health * -1;
        }
        else
        {
            focusedBodyPart.injuries.Add(new RWInjury(this, focusedBodyPart, 99f, damageType, ""));
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
                            DestroyBodyPart();
                            extraDamage = focusedBodyPart.health * -1;
                        }
                        else
                        {
                            focusedBodyPart.injuries.Add(new RWInjury(this, focusedBodyPart, 99f, damageType, ""));
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

        void DestroyBodyPart()
        {
            switch (focusedBodyPart.deathEffect)
            {
                case "Destroy":
                    List<RWBodyPart> subParts = new();

                    RWBodyPart focusedSubBodyPart = focusedBodyPart;

                    while (true)
                    {
                        for (int i = 0; i < bodyParts.Count; i++)
                        {
                            if (bodyParts[i].name == focusedSubBodyPart.subPartOf)
                            {
                                subParts.Add(bodyParts[i]);
                                focusedSubBodyPart = bodyParts[i];
                                break;
                            }
                            else
                            {
                                focusedSubBodyPart = null;
                            }
                        }

                        if (focusedSubBodyPart == null)
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < subParts.Count; i++)
                    {
                        bodyParts.Remove(subParts[i]);
                    }

                    focusedBodyPart.coverage = 0f;

                    focusedSubBodyPart.injuries.Clear();

                    focusedSubBodyPart.injuries.Add(new RWInjury(this, focusedSubBodyPart, 99f, damageType, ""));

                    break;
            }
        }
    }

    public List<RWBodyPart> bodyParts = new();

    public float maxHealth;

    public float bodySizeFactor;

    public float bloodLoss = 0;

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