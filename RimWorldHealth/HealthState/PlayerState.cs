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

        List<RWBodyPart> bodyParts = new(33) {
                new UpperTorso(this),
                new LowerTorso(this),

                new Neck(this),
                new Head(this),
                new Eye(this),
                new Ear(this),
                new Nose(this),
                new Jaw(this),
                new Tongue(this),

                new Shoulder(this),
                new Arm(this),
                new Hand(this),
                new Finger(this),

                new Leg(this),
                new Foot(this),
                new Toe(this),

                new Tail(this),

                new Skull(this),

                new Spine(this),
                new Ribcage(this),
                new Sternum(this),

                new Clavicle(this),
                new Humerus(this),
                new Radius(this),

                new Pelvis(this),

                new Femur(this),
                new Tibia(this),

                new Brain(this),

                new Stomach(this),
                new Heart(this),
                new Lung(this),
                new Kidney(this),
                new Liver(this),
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
                else if (bodyParts[i] is Ear)
                {
                    bodyPart = new Ear(this);
                }
                else if (bodyParts[i] is Shoulder)
                {
                    bodyPart = new Shoulder(this);
                }
                else if (bodyParts[i] is Arm)
                {
                    bodyPart = new Arm(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Hand)
                {
                    bodyPart = new Hand(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Leg)
                {
                    bodyPart = new Leg(this);
                }
                else if (bodyParts[i] is Foot)
                {
                    bodyPart = new Foot(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Clavicle)
                {
                    bodyPart = new Clavicle(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Humerus)
                {
                    bodyPart = new Humerus(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Radius)
                {
                    bodyPart = new Radius(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Femur)
                {
                    bodyPart = new Femur(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Tibia)
                {
                    bodyPart = new Tibia(this);

                    bodyPart.subPartOf = "Left " + bodyPart.subPartOf;

                    bodyParts[i].subPartOf = "Right " + bodyParts[i].subPartOf;
                }
                else if (bodyParts[i] is Lung)
                {
                    bodyPart = new Lung(this);
                }
                else if (bodyParts[i] is Kidney)
                {
                    bodyPart = new Kidney(this);
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
            else if (bodyParts[i] is Finger finger)
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 0; j < bodyParts[i].quantity/2; j++)
                    {
                        if (finger == null)
                        {
                            finger = new Finger(this);
                        }

                        if (j == 0)
                        {
                            finger.name = "Pinky";

                            finger.coverage = 6f;
                        }
                        else if (j == 1)
                        {
                            finger.name = "Ring Finger";

                            finger.coverage = 7f;
                        }
                        else if (j == 2)
                        {
                            finger.name = "Middle Finger";

                            finger.coverage = 8f;
                        }
                        else if (j == 3)
                        {
                            finger.name = "Index Finger";

                            finger.coverage = 7f;
                        }
                        else if (j == 4)
                        {
                            finger.name = "Thumb";

                            finger.coverage = 8f;
                        }

                        if (k == 0)
                        {
                            finger.name = "Right " + finger.name;
                            finger.subPartOf = "Right " + finger.subPartOf;
                            finger.group.Add("RightHand");
                        }
                        else
                        {
                            finger.name = "Left " + finger.name;
                            finger.subPartOf = "Left " + finger.subPartOf;
                            finger.group.Add("LeftHand");
                        }

                        this.bodyParts.Add(finger);

                        finger = null;
                    }
                }
            }
            else if (bodyParts[i] is Toe toe)
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 0; j < bodyParts[i].quantity / 2; j++)
                    {
                        if (toe == null)
                        {
                            toe = new Toe(this);
                        }

                        if (j == 0)
                        {
                            toe.name = "Little Toe";

                            toe.coverage = 6f;
                        }
                        else if (j == 1)
                        {
                            toe.name = "Fourth Toe";

                            toe.coverage = 7f;
                        }
                        else if (j == 2)
                        {
                            toe.name = "Middle Toe";

                            toe.coverage = 8f;
                        }
                        else if (j == 3)
                        {
                            toe.name = "Second Toe";

                            toe.coverage = 9f;
                        }
                        else if (j == 4)
                        {
                            toe.name = "Big Toe";

                            toe.coverage = 9f;
                        }

                        if (k == 0)
                        {
                            toe.name = "Right " + toe.name;
                            toe.subPartOf = "Right " + toe.subPartOf;
                            toe.group.Add("RightHand");
                        }
                        else
                        {
                            toe.name = "Left " + toe.name;
                            toe.subPartOf = "Left " + toe.subPartOf;
                            toe.group.Add("LeftHand");
                        }

                        this.bodyParts.Add(toe);

                        toe = null;
                    }
                }
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
        consciousness = 1;
        moving = 1;
        manipulation = 1;
        talking = 1;
        eating = 1;
        sight = 1;
        hearing = 1;
        breathing = 1;
        bloodFiltration = 1;
        bloodPumping = 1;
        digestion = 1;

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

        float brainEfficiency = 1;

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

                bloodLossPerCycle += injury.isBleeding && !injury.isTended ? injury.healingDifficulty.bleeding * injury.damage * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;

                if (heal)
                {
                    healList.Add(injury);
                }
            }

            bodyParts[i].efficiency = Mathf.Max(0, bodyParts[i].health / bodyParts[i].maxHealth);

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

        consciousness = brainEfficiency * (1 - Mathf.Clamp((pain - 0.1f) * 4 / 9, 0, 0.4f)) * (1 - 0.2f * (1 - bloodPumping)) *(1 - 0.2f * (1 - breathing)) *(1 - 0.1f * (1 - bloodFiltration));

        if (bloodLossPerCycle == 0 && bloodLoss > 0)
        {
            bloodLoss -= 33.3f / (40 * 60 * cycleLength);
        } //Replenishes 33.3% of blood per cycle if not bleeding
        else if (bloodLossPerCycle > 0)
        {
            bloodLoss += bloodLossPerCycle / 100 / (40 * 60 * cycleLength);
        }
        //bloodLoss math definetly is not right, will need to fix

        bloodLoss = Mathf.Clamp(bloodLoss, 0, 1);

        if (bloodLoss >= 0.6f)
        {
            consciousness -= 0.4f;

            consciousness = Mathf.Min(consciousness, 0.1f);
        }
        else if(bloodLoss >= 0.45f)
        {
            consciousness -= 0.4f;
        }
        else if (bloodLoss >= 0.3f)
        {
            consciousness -= 0.2f;
        }
        else if (bloodLoss >= 0.15f)
        {
            consciousness -= 0.1f;
        }

        Debug.Log("bloodlossPerCycle " + bloodLossPerCycle);
        Debug.Log("bloodloss " + bloodLoss);

        consciousness = Mathf.Max(consciousness, 0);

        moving *= Mathf.Min(1, consciousness);
        manipulation *= consciousness;
        talking *= consciousness;
        eating *= consciousness;

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

    public float consciousness = 1;

    public float moving = 1;

    public float manipulation = 1;

    public float talking = 1;

    public float eating = 1;

    public float sight = 1;

    public float hearing = 1;

    public float breathing = 1;

    public float bloodFiltration = 1;

    public float bloodPumping = 1;

    public float digestion = 1;

    public float cycleLength = 13;

    const int healingRateTics = 600;
    int healingRate = healingRateTics;
}