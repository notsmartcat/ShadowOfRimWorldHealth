using System.Collections.Generic;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

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

                    if (!armSetNames.Contains("Right"))
                    {
                        armSetNames.Add("Right");
                    }
                    if (!armSetNames.Contains("Left"))
                    {
                        armSetNames.Add("Left");
                    }

                    if (armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.shoulder = bodyParts[i] as Shoulder;
                    }
                    else
                    {
                        armSet.Add("Right", new ArmSet() { shoulder = bodyParts[i] as Shoulder });
                    }

                    if (armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.shoulder = bodyPart as Shoulder;
                    }
                    else
                    {
                        armSet.Add("Left", new ArmSet() { shoulder = bodyPart as Shoulder });
                    }
                }
                else if (bodyParts[i] is Arm)
                {
                    bodyPart = new Arm(this);

                    if (armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.arm = bodyParts[i] as Arm;
                    }
                    if (armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.arm = bodyPart as Arm;
                    }
                }
                else if (bodyParts[i] is Hand)
                {
                    bodyPart = new Hand(this);

                    if (armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.hand = bodyParts[i] as Hand;
                    }
                    if (armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.hand = bodyPart as Hand;
                    }
                }

                else if (bodyParts[i] is Leg)
                {
                    bodyPart = new Leg(this);

                    if (!legSetNames.Contains("Right"))
                    {
                        legSetNames.Add("Right");
                    }
                    if (!legSetNames.Contains("Left"))
                    {
                        legSetNames.Add("Left");
                    }

                    if (legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.leg = bodyParts[i] as Leg;
                    }
                    else
                    {
                        legSet.Add("Right", new LegSet() { leg = bodyParts[i] as Leg });
                    }

                    if (legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.leg = bodyPart as Leg;
                    }
                    else
                    {
                        legSet.Add("Left", new LegSet() { leg = bodyPart as Leg });
                    }
                }
                else if (bodyParts[i] is Foot)
                {
                    bodyPart = new Foot(this);

                    if (legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.foot = bodyParts[i] as Foot;
                    }
                    if (legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.foot = bodyPart as Foot;
                    }
                }

                else if (bodyParts[i] is Clavicle)
                {
                    bodyPart = new Clavicle(this);

                    if (armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.clavicle = bodyParts[i] as Clavicle;
                    }
                    if (armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.clavicle = bodyPart as Clavicle;
                    }
                }
                else if (bodyParts[i] is Humerus)
                {
                    bodyPart = new Humerus(this);

                    if (armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.humerus = bodyParts[i] as Humerus;
                    }
                    if (armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.humerus = bodyPart as Humerus;
                    }
                }
                else if (bodyParts[i] is Radius)
                {
                    bodyPart = new Radius(this);

                    if (armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.radius = bodyParts[i] as Radius;
                    }
                    if (armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.radius = bodyPart as Radius;
                    }
                }

                else if (bodyParts[i] is Femur)
                {
                    bodyPart = new Femur(this);

                    if (legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.femur = bodyParts[i] as Femur;
                    }
                    if (legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.femur = bodyPart as Femur;
                    }
                }
                else if (bodyParts[i] is Tibia)
                {
                    bodyPart = new Tibia(this);

                    if (legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.tibia = bodyParts[i] as Tibia;
                    }
                    if (legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.tibia = bodyPart as Tibia;
                    }
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
                    bodyPart.subName = "Left";

                    this.bodyParts.Add(bodyPart);
                }

                bodyParts[i].subName = "Right";
                this.bodyParts.Add(bodyParts[i]);
            }
            else if (bodyParts[i] is Finger finger)
            {
                for (int k = 0; k < armSetNames.Count; k++)
                {
                    for (int j = 0; j < bodyParts[i].quantity / armSetNames.Count; j++)
                    {
                        finger ??= new Finger(this);

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

                        finger.subName = armSetNames[k];
                        finger.group.Add(armSetNames[k] + "Hand");

                        if (armSet.TryGetValue(armSetNames[k], out ArmSet set))
                        {
                            set.fingers.Add(finger);
                        }

                        this.bodyParts.Add(finger);

                        finger = null;
                    }
                }
            }
            else if (bodyParts[i] is Toe toe)
            {
                for (int k = 0; k < legSetNames.Count; k++)
                {
                    for (int j = 0; j < bodyParts[i].quantity / legSetNames.Count; j++)
                    {
                        toe ??= new Toe(this);

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

                        toe.subName = legSetNames[k];

                        if (legSet.TryGetValue(legSetNames[k], out LegSet set))
                        {
                            set.toes.Add(toe);
                        }

                        this.bodyParts.Add(toe);

                        toe = null;
                    }
                }
            }
            else
            {
                this.bodyParts.Add(bodyParts[i]);
            }
        }

        for (int i = 0; i < this.bodyParts.Count; i++)
        {
            maxHealth += this.bodyParts[i].maxHealth;

            if (this.bodyParts[i] is Shoulder || this.bodyParts[i] is Arm || this.bodyParts[i] is Hand || this.bodyParts[i] is Finger || this.bodyParts[i] is Leg || this.bodyParts[i] is Foot || this.bodyParts[i] is Clavicle || this.bodyParts[i] is Humerus || this.bodyParts[i] is Radius || this.bodyParts[i] is Femur || this.bodyParts[i] is Tibia || this.bodyParts[i] is Toe)
            {
                continue;
            }

            if (this.bodyParts[i].capacity.Contains("Blood Filtration"))
            {
                bloodFiltrationBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Blood Pumping"))
            {
                bloodPumpingBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Breathing"))
            {
                breathingBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Digestion"))
            {
                digestionBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Eating"))
            {
                eatingBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Hearing"))
            {
                hearingBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Manipulation"))
            {
                manipulationBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Moving"))
            {
                movingBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Sight"))
            {
                sightBP.Add(this.bodyParts[i]);
            }
            if (this.bodyParts[i].capacity.Contains("Talking"))
            {
                talkingBP.Add(this.bodyParts[i]);
            }
        }
    }

    public void Update()
    {
        bloodLossPerCycle = 0;

        pain = 0;

        consciousness = 0;
        moving = 0;
        manipulation = 0;
        talking = 0;
        eating = 0;
        sight = 0;
        hearing = 0;
        breathing = 0;
        bloodFiltration = 0;
        bloodPumping = 0;
        digestion = 0;

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

        List<RWAffliction> afflictionList;

        for (int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].health = bodyParts[i].maxHealth;

            if (bodyParts[i].afflictions.Count == 0)
            {
                goto line1;
            }
            else if (IsSubPartDestroyed(this, bodyParts[i]))
            {
                bodyParts[i].health = 0;

                bodyParts[i].efficiency = 0;

                goto line1;
            }

            afflictionList = new(bodyParts[i].afflictions);

            for (int j = 0; j < afflictionList.Count; j++)
            {
                if (afflictionList[j] is RWDestroyed destroyed)
                {
                    bodyParts[i].health = 0;

                    bodyParts[i].efficiency = 0;

                    bloodLossPerCycle += destroyed.isBleeding && !destroyed.isTended ? destroyed.healingDifficulty.bleeding * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;

                    break;
                }

                if (afflictionList[j] is not RWInjury injury)
                {
                    if (afflictionList[j] is RWDisease disease)
                    {
                        Disease(disease, bodyParts[i]);
                    }

                    continue;
                }

                bodyParts[i].health -= injury.damage;

                if (dead)
                {
                    continue;
                }

                if (injury is RWScar scar && scar.isRevealed)
                {
                    if (scar.painCategory == "painful")
                    {
                        afflictionList[j].pain = scar.scarDamage * 1.5f * injury.healingDifficulty.scarPain / bodySizeFactor / 100;
                    }
                    else if (scar.painCategory == "aching")
                    {
                        afflictionList[j].pain = scar.scarDamage * injury.healingDifficulty.scarPain / bodySizeFactor / 100;
                    }
                    else if (scar.painCategory == "itchy")
                    {
                        afflictionList[j].pain = scar.scarDamage * 0.5f * injury.healingDifficulty.scarPain / bodySizeFactor / 100;
                    }
                }
                else
                {
                    afflictionList[j].pain = injury.damage * injury.healingDifficulty.pain / bodySizeFactor / 100;
                }

                pain += afflictionList[j].pain;

                bloodLossPerCycle += injury.isBleeding && !injury.isTended ? injury.healingDifficulty.bleeding * injury.damage * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;

                if (injury.infectionTimer > 0)
                {
                    injury.infectionTimer--;

                    if (injury.infectionTimer <= 0)
                    {
                        float infectionChance = injury.healingDifficulty.infectionChance * (injury.isTended ? Mathf.Lerp(0.85f, 0.05f , injury.tendQuality) : 1);

                        if (creature.Room != null && creature.Room.shelter)
                        {
                            infectionChance *= 0.5f;
                        }

                        float random = Random.value;

                        if (random < infectionChance)
                        {
                            bodyParts[i].afflictions.Add(new RWInfection(this, bodyParts[i]));
                        }
                    }
                }

                if (heal)
                {
                    healList.Add(injury);
                }
            }

        line1:

            bodyParts[i].efficiency = Mathf.Max(0, bodyParts[i].health / bodyParts[i].maxHealth);

            if ((bodyParts[i].health > 0 && bodyParts[i].health < 1) || bodyParts[i].deathEffect == "" && bodyParts[i].health < 1)
            {
                bodyParts[i].health = 1;
                bodyParts[i].efficiency = 0;
            }
            else if (bodyParts[i] is UpperTorso && bodyParts[i].health <= 0)
            {
                bodyParts[i].health = 0;
                bodyParts[i].efficiency = 0;
            }

            if (dead)
            {
                continue;
            }

            if (bodyParts[i] is Brain)
            {
                brainEfficiency = bodyParts[i].efficiency;
            }
        }

        afflictionList = new(wholeBodyAfflictions);

        for (int i = 0; i < afflictionList.Count; i++)
        {
            if (afflictionList[i] is RWDisease disease)
            {
                Disease(disease);
            }
        }

        if (dead)
        {
            return;
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

            if (injury is RWScar scar)
            {
                if (scar.damage <= scar.scarDamage)
                {
                    scar.damage = scar.scarDamage;
                    scar.isTended = true;
                    scar.isBleeding = false;
                    scar.isRevealed = true;

                    scar.healingDifficulty.combines = false;
                }
            }
            else if (injury.damage <= 0)
            {
                injury.healingDifficulty = null;
                injury.part.afflictions.Remove(injury);
            }
        }

        if (bloodFiltrationBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = bloodFiltration;
            float postFactors = 1;

            for (int i = 0; i < bloodFiltrationBP.Count; i++)
            {
                baseEfficiency += (bloodFiltrationBP[i] is Kidney ? (bloodFiltrationBP[i].efficiency / 2) : bloodFiltrationBP[i].efficiency) / (bloodFiltrationBP.Count != 1 ? bloodFiltrationBP.Count - 1 : bloodFiltrationBP.Count);
            }

            bloodFiltration = (baseEfficiency + offsets) * postFactors;
        }
        else
        {
            bloodFiltration = 0;
        }

        if (bloodPumpingBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = bloodPumping;
            float postFactors = 1;

            for (int i = 0; i < bloodPumpingBP.Count; i++)
            {
                baseEfficiency += bloodPumpingBP[i].efficiency / bloodPumpingBP.Count;
            }

            bloodPumping = (baseEfficiency + offsets) * postFactors;
        }
        else
        {
            bloodPumping = 0;
        }

        if (breathingBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = breathing;
            float postFactors = 1;

            for (int i = 0; i < breathingBP.Count; i++)
            {
                baseEfficiency += (breathingBP[i] is Lung ? (breathingBP[i].efficiency / 2) : breathingBP[i].efficiency) / (breathingBP.Count != 1 ? (breathingBP.Count - 1) : breathingBP.Count);
            }

            breathing = (baseEfficiency + offsets) * postFactors;
        }
        else
        {
            breathing = 0;
        }

        float consciounessOffset = consciousness;

        consciousness = (brainEfficiency * (1 - Mathf.Clamp((pain - 0.1f) * 4 / 9, 0, 0.4f)) * (1 - 0.2f * (1 - bloodPumping)) * (1 - 0.2f * (1 - breathing)) * (1 - 0.1f * (1 - bloodFiltration))) + consciounessOffset;

        if (bloodLossPerCycle == 0 && bloodLoss > 0)
        {
            bloodLoss -= 0.333f / (40 * 60 * cycleLength);
        } //Replenishes 33.3% of blood per cycle if not bleeding
        else if (bloodLossPerCycle > 0)
        {
            bloodLoss += bloodLossPerCycle / 100 / (40 * 60 * cycleLength);
        }

        bloodLoss = Mathf.Clamp(bloodLoss, 0, 1);

        if (bloodLoss >= 0.6f)
        {
            consciousness -= 0.4f;

            forceUnconsciousness = true;
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

        if (forceUnconsciousness)
        {
            consciousness = Mathf.Min(consciousness, 0.1f);
        }

        consciousness = Mathf.Max(consciousness, 0);

        if (digestionBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = digestion;
            float postFactors = 1;

            for (int i = 0; i < digestionBP.Count; i++)
            {
                baseEfficiency += ((digestionBP[i] is Stomach || digestionBP[i] is Liver) ? digestionBP[i].efficiency / 2 : digestionBP[i].efficiency) / (digestionBP.Count != 1 ? digestionBP.Count - 1 : digestionBP.Count);
            }

            digestion = (baseEfficiency + offsets) * postFactors;
        }
        else
        {
            digestion = 0;
        }

        if (eatingBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = eating;
            float postFactors = 1;

            for (int i = 0; i < eatingBP.Count; i++)
            {
                baseEfficiency += eatingBP[i].efficiency / eatingBP.Count;
            }

            eating = (baseEfficiency + offsets) * postFactors;
        }
        else
        {
            eating = 0;
        }

        if (hearingBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = hearing;
            float postFactors = 1;

            if (hearingBP.Count == 1)
            {
                baseEfficiency = hearingBP[0].efficiency;

                hearing = (baseEfficiency + offsets) * postFactors;
            }
            else
            {
                float bestEfficiency = 0;

                for (int i = 0; i < hearingBP.Count; i++)
                {
                    baseEfficiency += hearingBP[i].efficiency / (hearingBP.Count * 2);

                    if (hearingBP[i].efficiency > bestEfficiency)
                    {
                        bestEfficiency = hearingBP[i].efficiency;
                    }
                }

                baseEfficiency += bestEfficiency / 2;

                hearing = (baseEfficiency + offsets) * postFactors;
            }
        }
        else
        {
            hearing = 0;
        }

        if ((armSetNames.Count + manipulationBP.Count) > 0)
        {
            float baseEfficiency = 0;
            float offsets = manipulation;
            float postFactors = 1;

            for (int i = 0; i < manipulationBP.Count; i++)
            {
                postFactors *= manipulationBP[i].efficiency;
            }

            for (int i = 0; i < armSetNames.Count; i++)
            {
                baseEfficiency += armSet[armSetNames[i]].Efficiency(offsets, postFactors) / armSetNames.Count;
            }

            manipulation = baseEfficiency;
        }
        else
        {
            manipulation = 0;
        }

        if ((legSetNames.Count + movingBP.Count) > 0)
        {
            float baseEfficiency = 0;
            float offsets = moving;
            float postFactors = 1;

            for (int i = 0; i < movingBP.Count; i++)
            {
                postFactors *= movingBP[i].efficiency;
            }

            for (int i = 0; i < legSetNames.Count; i++)
            {
                baseEfficiency += legSet[legSetNames[i]].Efficiency(this, offsets, postFactors) / legSetNames.Count;
            }

            moving = baseEfficiency;
        }
        else
        {
            moving = 0;
        }

        if (sightBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = sight;
            float postFactors = 1;

            if (sightBP.Count == 1)
            {
                baseEfficiency = sightBP[0].efficiency;

                sight = (baseEfficiency + offsets) * postFactors;
            }
            else
            {
                float bestEfficiency = 0;

                for (int i = 0; i < sightBP.Count; i++)
                {
                    baseEfficiency += sightBP[i].efficiency / (sightBP.Count * 2);

                    if (sightBP[i].efficiency > bestEfficiency)
                    {
                        bestEfficiency = sightBP[i].efficiency;
                    }
                }

                baseEfficiency += bestEfficiency / 2;

                sight = (baseEfficiency + offsets) * postFactors;
            }
        }
        else
        {
            sight = 0;
        }

        if (talkingBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = talking;
            float postFactors = 1;

            for (int i = 0; i < talkingBP.Count; i++)
            {
                baseEfficiency += talkingBP[i].efficiency / talkingBP.Count;
            }

            talking = (baseEfficiency + offsets) * postFactors;
        }
        else
        {
            talking = 0;
        }

        manipulation *= consciousness;
        talking *= consciousness;
        eating *= consciousness;

        eating = Mathf.Max(0.1f, eating);

        void Disease(RWDisease disease, RWBodyPart part = null)
        {
            if (!disease.isImmune)
            {
                disease.severity += disease.severityGain * disease.InfectionLuck / (40 * 60 * cycleLength);

                if (disease.isTended)
                {
                    disease.severity -= disease.treatment * disease.tendQuality / (40 * 60 * cycleLength);
                }

                disease.immunity += disease.immunityGain / (40 * 60 * cycleLength);

                disease.timeUntilTreatment -= 1 / (40 * 60 * cycleLength);

                if (disease.timeUntilTreatment <= -3)
                {
                    disease.isTended = false;
                }
            }
            else
            {
                disease.severity -= disease.severityLoss / (40 * 60 * cycleLength);
            }

            if (disease.isImmune && disease.severity <= 0)
            {
                if (part == null)
                {
                    wholeBodyAfflictions.Remove(disease);
                }
                else
                {
                    part.afflictions.Remove(disease);
                }

                return;
            }

            if (disease is RWFlu)
            {
                if (disease.severity <= 0.665f)
                {
                    consciousness -= 0.05f;
                    manipulation -= 0.05f;
                    breathing -= 0.1f;
                }
                else if (disease.severity <= 0.832f)
                {
                    consciousness -= 0.1f;
                    manipulation -= 0.1f;
                    breathing -= 0.15f;
                }
                else
                {
                    pain += 0.05f;

                    consciousness -= 0.15f;
                    manipulation -= 0.2f;
                    breathing -= 0.2f;
                }
            }
            else if (disease is RWInfection)
            {
                if (disease.severity <= 0.32f)
                {
                    pain += 0.05f;
                }
                else if (disease.severity <= 0.77f)
                {
                    pain += 0.08f;
                }
                else if (disease.severity <= 0.86f)
                {
                    pain += 0.12f;

                    consciousness -= 0.5f;
                }
                else
                {
                    forceUnconsciousness = true;

                    pain += 0.85f;

                    breathing -= 0.5f;
                }
            }
        }
    }

    public void Damage(RWDamageType damageType, float damage, RWBodyPart bodyPart, string attackName = "", string attackerName = "")
    {
        RWBodyPart focusedBodyPart = bodyPart;

        float health = focusedBodyPart.health;
        health -= damage;

        float extraDamage = 0f;

        OverkillPrevention();

        if (damage <= 0)
        {
            return;
        }

        if (health <= 0f)
        {
            DestroyBodyPart();

            extraDamage = health * -1;
            damage = 0;

            focusedBodyPart.health = 0;
        }
        else
        {
            focusedBodyPart.afflictions.Add(Scar(damage));
            focusedBodyPart.health = health;
        }

        if (damageType.armourCategory == "Blunt")
        {
            damage = 0;
        }

        while (extraDamage > 0 || damage > 0)
        {
            bool bodypartHit = false;

            if (focusedBodyPart.isInternal && focusedBodyPart.subPartOf != "")
            {
                for (int i = 0; i < bodyParts.Count; i++)
                {
                    if (IsSubPartName(focusedBodyPart, bodyParts[i]))
                    {
                        focusedBodyPart = bodyParts[i];

                        health = focusedBodyPart.health;
                        health -= damage + extraDamage;

                        OverkillPrevention();

                        float tempDamage = damage + extraDamage;

                        if (health <= 0f)
                        {
                            DestroyBodyPart();
                            extraDamage = health * -1;
                            damage = 0;

                            focusedBodyPart.health = 0;
                        }
                        else
                        {
                            extraDamage = 0f;

                            focusedBodyPart.afflictions.Add(Scar(tempDamage));
                            focusedBodyPart.health = health;
                        }

                        bodypartHit = true;

                        break;
                    }
                }
            }
            else if (extraDamage > 0 && damageType is RWBomb && BombDestroyBodyparts())
            {
                for (int i = 0; i < bodyParts.Count; i++)
                {
                    if (IsSubPartName(focusedBodyPart, bodyParts[i]))
                    {
                        focusedBodyPart = bodyParts[i];

                        health = focusedBodyPart.health;
                        health -= damage + extraDamage;

                        OverkillPrevention();

                        float tempDamage = damage + extraDamage;

                        if (health <= 0f)
                        {
                            DestroyBodyPart();
                            extraDamage = health * -1;
                            damage = 0;

                            focusedBodyPart.health = 0;
                        }
                        else
                        {
                            extraDamage = 0f;

                            focusedBodyPart.afflictions.Add(Scar(tempDamage));
                            focusedBodyPart.health = health;
                        }

                        bodypartHit = true;

                        break;
                    }
                }
            }
            else if (extraDamage > 0 && damageType is RWSuperBomb)
            {
                for (int i = 0; i < bodyParts.Count; i++)
                {
                    if (IsSubPartName(focusedBodyPart, bodyParts[i]))
                    {
                        focusedBodyPart = bodyParts[i];

                        health = focusedBodyPart.health;
                        health -= damage + extraDamage;

                        OverkillPrevention();

                        float tempDamage = damage + extraDamage;

                        if (health <= 0f)
                        {
                            DestroyBodyPart();
                            extraDamage = health * -1;
                            damage = 0;

                            focusedBodyPart.health = 0;
                        }
                        else
                        {
                            extraDamage = 0f;

                            focusedBodyPart.afflictions.Add(Scar(tempDamage));
                            focusedBodyPart.health = health;
                        }

                        bodypartHit = true;

                        break;
                    }
                }
            }
            else
            {
                break;
            }

            if (!bodypartHit)
            {
                break;
            }
        }

        void DestroyBodyPart()
        {
            if (focusedBodyPart == null || focusedBodyPart.deathEffect == "" || focusedBodyPart is UpperTorso)
            {
                return;
            }

            if (true || focusedBodyPart.deathEffect == "Destroy" || focusedBodyPart.deathEffect == "Death" || focusedBodyPart.deathEffect == "CutInHalf" || focusedBodyPart.deathEffect == "Decapitation")
            {
                List<RWBodyPart> subParts = new()
                {
                    focusedBodyPart
                };

                List<RWBodyPart> subPartsRestricted = new();

                while (true)
                {
                    bool newBodyParts = false;

                    for (int i = 0; i < bodyParts.Count; i++)
                    {
                        for (int j = 0; j < subParts.Count; j++)
                        {
                            if (!subParts.Contains(bodyParts[i]) && !subPartsRestricted.Contains(bodyParts[i]) && IsSubPartName(bodyParts[i], subParts[j]))
                            {
                                if (subParts[j].afflictions.Count == 1 && subParts[j].afflictions[0] is RWDestroyed || subParts[j].deathEffect == "")
                                {
                                    subPartsRestricted.Add(bodyParts[i]);
                                    continue;
                                }

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
                    subParts[j].afflictions.Add(new RWDestroyed(this, subParts[j], 0f, damageType, attackName, attackerName));
                }
            }
        }

        void OverkillPrevention()
        {
            if (health > 0)
            {
                return;
            }

            float overkillPercentage = health * -1 / focusedBodyPart.maxHealth * 100;

            float chanceToDestroy = (overkillPercentage - damageType.overkillMin) / (damageType.overkillMax - damageType.overkillMin);

            if (Random.value > chanceToDestroy)
            {
                damage = (damage + health) - 1;
                health = 1;
            }
        }

        RWInjury Scar(float damage)
        {
            if (damageType.headiffs.Count >= 2 && damageType.headiffs[2] == "Bruise" && !focusedBodyPart.isSolid && !focusedBodyPart.isInternal || damageType.armourCategory == "Blunt" && focusedBodyPart is RWOrgan && !focusedBodyPart.isDelicate)
            {
                return new(this, focusedBodyPart, damage, damageType, attackName, attackerName);
            } //bruises never scar

            float oddsOfScarring;

            if (focusedBodyPart is Brain)
            {
                oddsOfScarring = 100;
            }
            else if (focusedBodyPart.isDelicate)
            {
                oddsOfScarring = 50f * Mathf.Clamp((damage - 1) / 5, 0, 1);
            }
            else if(focusedBodyPart.isSolid)
            {
                oddsOfScarring = 1f * Mathf.Clamp((damage - 4) / 10, 0, 1);
            }
            else
            {
                oddsOfScarring = 2f * Mathf.Clamp((damage - 4) / 10, 0, 1);
            }

            if ((Random.value * 100) >= oddsOfScarring)
            {
                return new(this, focusedBodyPart, damage, damageType, attackName, attackerName);
            }

            if (damage < 1)
            {
                damage = 1;
            }

            RWScar scar = new(this, focusedBodyPart, damage, damageType, attackName, attackerName);

            float pain = Random.value;

            if (pain > 0.9f)
            {
                scar.painCategory = "painful";
            }
            else if (pain > 0.7f)
            {
                scar.painCategory = "aching";
            }
            else if (pain > 0.5f)
            {
                scar.painCategory = "itchy";
            }

            if (focusedBodyPart.isDelicate)
            {
                scar.isPermanent = true;
                scar.isRevealed = true;

                scar.isTended = true;
                scar.isBleeding = false;

                scar.healingDifficulty.combines = false;
            }

            if (scar.isPermanent)
            {
                scar.scarDamage = damage;
            }
            else
            {
                scar.scarDamage = Mathf.Lerp(1, damage / 2, Random.value);
            }

            return scar;
        }

        bool BombDestroyBodyparts()
        {
            if (focusedBodyPart is Finger || focusedBodyPart is Hand || focusedBodyPart is Toe || focusedBodyPart is Foot || focusedBodyPart is Ear || focusedBodyPart is Jaw || focusedBodyPart is Eye)
            {
                return true;
            }

            return false;
        }
    }

    public int BloodLossMultiplier(RWBodyPart part)
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

    public List<RWBodyPart> bodyParts = new();

    public List<RWAffliction> wholeBodyAfflictions = new();

    readonly List<string> armSetNames = new();
    readonly Dictionary<string, ArmSet> armSet = new();
    readonly List<string> legSetNames = new();
    readonly Dictionary<string, LegSet> legSet = new();

    readonly List<RWBodyPart> bloodFiltrationBP = new();
    readonly List<RWBodyPart> bloodPumpingBP = new();
    readonly List<RWBodyPart> breathingBP = new();
    readonly List<RWBodyPart> digestionBP = new();
    readonly List<RWBodyPart> eatingBP = new();
    readonly List<RWBodyPart> hearingBP = new();
    readonly List<RWBodyPart> manipulationBP = new();
    readonly List<RWBodyPart> movingBP = new();
    readonly List<RWBodyPart> sightBP = new();
    readonly List<RWBodyPart> talkingBP = new();

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

    public bool forceUnconsciousness = false;

    const int healingRateTics = 600;
    int healingRate = healingRateTics;
}