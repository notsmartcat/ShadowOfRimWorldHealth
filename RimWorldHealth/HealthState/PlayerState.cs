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
                new Neck(this),
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
            if (bodyParts[i].injuries.Count == 0)
            {
                continue;
            }

            bodyParts[i].health = bodyParts[i].maxHealth;

            for (int j = 0; j < bodyParts[i].injuries.Count; j++)
            {
                if (bodyParts[i].injuries[j] is Destroyed)
                {
                    bodyParts[i].health = 0;
                    break;
                }

                bodyParts[i].health -= bodyParts[i].injuries[j].damage;

                if ((bodyParts[i].health > 0 && bodyParts[i].health < 1) || bodyParts[i].deathEffect == "" && bodyParts[i].health < 1)
                {
                    bodyParts[i].health = 1;
                }
            }

            bodyParts[i].efficiency = Mathf.Floor(bodyParts[i].health / bodyParts[i].maxHealth * 100);
        }
    }

    public void Damage(string damageType, float damage, RWBodyPart bodyPart, string attackerName = "")
    {
        RWBodyPart focusedBodyPart = bodyPart;

        float health = focusedBodyPart.health;
        health -= damage;

        float extraDamage = 0f;

        Debug.Log(focusedBodyPart.name + " was hit for " + damage);

        if (health < 0f)
        {
            if(focusedBodyPart.deathEffect != "")
                DestroyBodyPart();
            extraDamage = health * -1;
        }
        else
        {
            focusedBodyPart.injuries.Add(new RWInjury(this, focusedBodyPart, damage, damageType, attackerName));
        }

        while (true)
        {
            if (focusedBodyPart.isInternal && focusedBodyPart.subPartOf != "")
            {
                for (int i = 0; i < bodyParts.Count; i++)
                {
                    if (bodyParts[i].name == focusedBodyPart.subPartOf)
                    {
                        focusedBodyPart = bodyParts[i];

                        health = focusedBodyPart.health;
                        health -= damage + extraDamage;

                        Debug.Log(focusedBodyPart.name + " was hit for " + damage + " damage and " + extraDamage + " extraDamage now it's health is " + health);

                        extraDamage = 0f;

                        if (health < 0f)
                        {
                            DestroyBodyPart();
                            extraDamage = health * -1;
                        }
                        else
                        {
                            focusedBodyPart.injuries.Add(new RWInjury(this, focusedBodyPart, damage, damageType, attackerName));
                        }

                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }

        void DestroyBodyPart()
        {
            if (focusedBodyPart.deathEffect == "Destroy" || focusedBodyPart.deathEffect == "Death" || focusedBodyPart.deathEffect == "CutInHalf" || focusedBodyPart.deathEffect == "Decapitation" || focusedBodyPart.deathEffect == "")
            {
                List<RWBodyPart> subParts = new()
                {
                    focusedBodyPart
                };

                List<RWBodyPart> subPartsRestricted = new();

                Debug.Log("Destroying body part = " + focusedBodyPart.name);

                while (true)
                {
                    bool newBodyParts = false;

                    for (int i = 0; i < bodyParts.Count; i++)
                    {
                        for (int j = 0; j < subParts.Count; j++)
                        {
                            if (!subParts.Contains(bodyParts[i]) && !subPartsRestricted.Contains(bodyParts[i]) && bodyParts[i].subPartOf == subParts[j].name)
                            {
                                if (subParts[j].injuries.Count == 1)
                                {
                                    if (subParts[j].injuries[0] is Destroyed)
                                    {
                                        Debug.Log("Tried to destroy subBody part = " + subParts[j].name + " but it was already destroyed");

                                        subPartsRestricted.Add(bodyParts[i]);
                                        continue;
                                    }
                                }

                                Debug.Log("Destroying subBody part = " + bodyParts[i].name);

                                newBodyParts = true;
                                subParts.Add(bodyParts[i]);
                            }
                        }

                    }

                    if (!newBodyParts)
                    {
                        break;
                    }
                }

                for (int j = 0; j < subParts.Count; j++)
                {
                    subParts[j].injuries.Clear();
                    subParts[j].injuries.Add(new Destroyed(this, subParts[j], 0f, damageType, attackerName));
                }
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