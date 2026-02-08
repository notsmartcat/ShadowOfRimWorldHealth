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
                new Eye(this),
                new Ribcage(this),
                new Heart(this)
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
        bloodLossPerCycle = 0;

        pain = 0;
        consciousness = 100;
        moving = 100;
        manipulation = 100;
        talking = 100;
        eating = 100;
        sight = 100;
        hearing = 100;
        breathing = 100;
        bloodFiltration = 100;
        bloodPumping = 100;
        digestion = 100;

        if (healingRate > 0)
        {
            healingRate--;
        }

        bool heal = healingRate <= 0;

        if (heal)
        {
            healingRate = healingRateTics;
        }

        List<RWInjury> healList = new();

        float brainEfficiency = 100;

        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (bodyParts[i].afflictions.Count == 0)
            {
                continue;
            }

            bodyParts[i].health = bodyParts[i].maxHealth;

            for (int j = 0; j < bodyParts[i].afflictions.Count; j++)
            {
                if (bodyParts[i].afflictions[j] is RWDestroyed destroyed)
                {
                    bodyParts[i].health = 0;

                    bloodLossPerCycle += !bodyParts[i].isInternal && destroyed.isBleeding ? 12 * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;
                    break;
                }

                if (bodyParts[i].afflictions[j] is not RWInjury injury)
                {
                    return;
                }

                bodyParts[i].health -= injury.damage;

                bodyParts[i].afflictions[j].pain = injury.damage * injury.healingDifficulty.pain / bodySizeFactor;

                pain += bodyParts[i].afflictions[j].pain;

                bloodLossPerCycle += injury.isBleeding ? injury.healingDifficulty.bleeding * injury.damage * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;

                if (heal)
                {
                    healList.Add(injury);
                }
            }

            bodyParts[i].efficiency = Mathf.Max(0, Mathf.Floor(bodyParts[i].health / bodyParts[i].maxHealth * 100));

            if ((bodyParts[i].health > 0 && bodyParts[i].health < 1) || bodyParts[i].deathEffect == "" && bodyParts[i].health < 1)
            {
                bodyParts[i].health = 1;
            }
            else if (bodyParts[i] is UpperTorso && bodyParts[i].health <= 0)
            {
                bodyParts[i].health = 0;
            }

            if (bodyParts[i] is Brain)
            {
                brainEfficiency = bodyParts[i].efficiency;
            }
        }

        if (healList.Count > 0)
        {
            RWInjury injury = healList[Random.Range(0, healList.Count)];

            float healRate = 8;

            if (injury.isTended)
            {
                healRate += 4;

                healRate += Mathf.Floor(injury.tendQuality) * 0.08f;
            }

            injury.damage -= healRate * 0.1f;

            if (injury.damage <= 0)
            {
                injury.healingDifficulty = null;
                injury.part.afflictions.Remove(injury);
            }
        }

        consciousness = brainEfficiency * (1 - Mathf.Clamp((pain/100 - 0.1f) * 4 / 9, 0, 0.4f)) * (1 - 0.2f * (1 - bloodPumping/100)) *(1 - 0.2f * (1 - breathing/100)) *(1 - 0.1f * (1 - bloodFiltration/100));

        if (bloodLossPerCycle == 0 && bloodLoss > 0)
        {
            bloodLoss -= cycleLength / 33.3f;
        } //Replenishes 33.3% of blood per cycle if not bleeding
        else if(bloodLossPerCycle > 0)
        {
            bloodLoss += cycleLength / bloodLossPerCycle;
        }
        //bloodLoss math definetly is not right, will need to fix

        bloodLoss = Mathf.Clamp(bloodLoss, 0, 100);

        if (bloodLoss >= 60)
        {
            consciousness -= 40;

            consciousness = Mathf.Min(consciousness, 10);
        }
        else if(bloodLoss >= 45)
        {
            consciousness -= 40;
        }
        else if (bloodLoss >= 30)
        {
            consciousness -= 20;
        }
        else if (bloodLoss >= 15)
        {
            consciousness -= 10;
        }

        //Debug.Log(bloodLoss);

        consciousness = Mathf.Max(consciousness, 0);

        moving *= Mathf.Min(1, consciousness / 100);
        manipulation *= consciousness / 100;
        talking *= consciousness / 100;
        eating *= consciousness / 100;

        static int BloodLossMultiplier(RWBodyPart part)
        {
            int multiplier = 1;

            if (part is Neck)
            {
                multiplier = 3;
            }
            else if (part is Heart)
            {
                multiplier = 5;
            }

            return multiplier;
        }
    }

    public void Damage(RWDamageType damageType, float damage, RWBodyPart bodyPart, string attackerName = "")
    {
        RWBodyPart focusedBodyPart = bodyPart;

        float health = focusedBodyPart.health;
        health -= damage;

        float extraDamage = 0f;

        Debug.Log(focusedBodyPart.name + " was hit for " + damage + " damage, it's health is now " + health);

        if (health < 0f)
        {
            if(focusedBodyPart.deathEffect != "" && focusedBodyPart is not UpperTorso)
                DestroyBodyPart();

            extraDamage = health * -1;
        }
        else
        {
            focusedBodyPart.afflictions.Add(new RWInjury(this, focusedBodyPart, damage, damageType, attackerName));
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
                            focusedBodyPart.afflictions.Add(new RWInjury(this, focusedBodyPart, damage, damageType, attackerName));
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

                Debug.Log("Destroying body part " + focusedBodyPart.name);

                while (true)
                {
                    bool newBodyParts = false;

                    for (int i = 0; i < bodyParts.Count; i++)
                    {
                        for (int j = 0; j < subParts.Count; j++)
                        {
                            if (!subParts.Contains(bodyParts[i]) && !subPartsRestricted.Contains(bodyParts[i]) && bodyParts[i].subPartOf == subParts[j].name)
                            {
                                if (subParts[j].afflictions.Count == 1)
                                {
                                    if (subParts[j].afflictions[0] is RWDestroyed)
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
                    subParts[j].afflictions.Clear();
                    subParts[j].afflictions.Add(new RWDestroyed(this, subParts[j], 0f, damageType, attackerName));
                }
            }
        }
    }

    public List<RWBodyPart> bodyParts = new();

    public List<RWAffliction> wholeBodyAfflictions = new();

    public float maxHealth;

    public float bodySizeFactor = 1;

    public float bloodLoss = 0;
    public float bloodLossPerCycle = 0;

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

    public float cycleLength = 13;

    const int healingRateTics = 600;
    int healingRate = healingRateTics;
}