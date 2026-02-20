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

        List<RWBodyPart> bloodFiltrationBP = new();
        List<RWBodyPart> bloodPumpingBP = new();
        List<RWBodyPart> breathingBP = new();
        List<RWBodyPart> digestionBP = new();
        List<RWBodyPart> eatingBP = new();
        List<RWBodyPart> hearingBP = new();
        List<RWBodyPart> manipulationBP = new();
        List<RWBodyPart> movingBP = new();
        List<RWBodyPart> sightBP = new();
        List<RWBodyPart> talkingBP = new();

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

            for (int j = 0; j < bodyParts[i].afflictions.Count; j++)
            {
                if (bodyParts[i].afflictions[j] is RWDestroyed destroyed)
                {
                    bodyParts[i].health = 0;

                    bodyParts[i].efficiency = 0;

                    bloodLossPerCycle += destroyed.isBleeding && !destroyed.isTended ? destroyed.healingDifficulty.bleeding * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;

                    break;
                }

                if (bodyParts[i].afflictions[j] is not RWInjury injury)
                {
                    return;
                }

                bodyParts[i].health -= injury.damage;

                if (dead)
                {
                    continue;
                }

                bodyParts[i].afflictions[j].pain = injury.damage * injury.healingDifficulty.pain / bodySizeFactor;

                pain += bodyParts[i].afflictions[j].pain;

                bloodLossPerCycle += injury.isBleeding && !injury.isTended ? injury.healingDifficulty.bleeding * injury.damage * bodySizeFactor * BloodLossMultiplier(bodyParts[i]) : 0;

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

            for (int c = 0; c < bodyParts[i].capacity.Count; c++)
            {
                if (bodyParts[i].capacity[c] == "Blood Filtration")
                {
                    bloodFiltrationBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Blood Pumping")
                {
                    bloodPumpingBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Breathing")
                {
                    breathingBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Digestion")
                {
                    digestionBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Eating")
                {
                    eatingBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Hearing")
                {
                    hearingBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Manipulation")
                {
                    manipulationBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Moving")
                {
                    movingBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Sight")
                {
                    sightBP.Add(bodyParts[i]);
                }
                else if (bodyParts[i].capacity[c] == "Talking")
                {
                    talkingBP.Add(bodyParts[i]);
                }
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

            if (injury.damage <= 0)
            {
                injury.healingDifficulty = null;
                injury.part.afflictions.Remove(injury);
            }
        }

        if (bloodFiltrationBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = 0;
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
            float offsets = 0;
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
            float offsets = 0;
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

        consciousness = brainEfficiency * (1 - Mathf.Clamp((pain - 0.1f) * 4 / 9, 0, 0.4f)) * (1 - 0.2f * (1 - bloodPumping)) *(1 - 0.2f * (1 - breathing)) *(1 - 0.1f * (1 - bloodFiltration));

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

        //Debug.Log("bloodlossPerCycle " + bloodLossPerCycle);
        //Debug.Log("bloodloss " + bloodLoss);

        consciousness = Mathf.Max(consciousness, 0);

        if (digestionBP.Count > 0)
        {
            float baseEfficiency = 0;
            float offsets = 0;
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
            float offsets = 0;
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
            float offsets = 0;
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

        if (manipulationBP.Count > 0)
        {
            Dictionary<string, List<RWBodyPart>> limbs = new();
            List<string> limbNames = new();

            float offsets = 0;
            float postFactors = 1;

            for (int j = 0; j < manipulationBP.Count; j++)
            {
                string Temp = "";
                for (int i = 0; i < manipulationBP[j].name.Length; i++)
                {
                    char letter = manipulationBP[j].name[i];

                    if (letter.ToString() == " ")
                    {
                        if (limbs.TryGetValue(Temp, out _))
                        {
                            limbs[Temp].Add(manipulationBP[j]);
                        }
                        else
                        {
                            limbs[Temp] = new List<RWBodyPart>(1) { manipulationBP[j] };
                            limbNames.Add(Temp);

                            //Debug.Log("new LimbName " + Temp);
                        }

                        break;
                    }
                    else
                    {
                        Temp += letter;
                    }
                }
            }

            for (int i = 0; i < limbNames.Count; i++)
            {
                float armlimbEfficiency = 0;
                float shoulderEfficiency = 0;
                float clavicleEfficiency = 0;
                float humerusEfficiency = 0;
                float radiusEfficiency = 0;
                float handEfficiency = 0;
                float fingerEfficiency = 0;

                int fingerNumber = 0;

                for (int j = 0; j < limbs[limbNames[i]].Count; j++)
                {
                    if (limbs[limbNames[i]][j] is Arm)
                    {
                        armlimbEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Shoulder)
                    {
                        shoulderEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Clavicle)
                    {
                        clavicleEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Humerus)
                    {
                        humerusEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Radius)
                    {
                        radiusEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Hand)
                    {
                        handEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Finger)
                    {
                        fingerEfficiency += limbs[limbNames[i]][j].efficiency;
                        fingerNumber++;
                    }
                }

                float wholeArmEfficiency = (armlimbEfficiency) * (shoulderEfficiency) * (clavicleEfficiency) * (humerusEfficiency) * (radiusEfficiency) * (handEfficiency) * ((fingerEfficiency * (0.8f / fingerNumber)) + 0.2f);

                wholeArmEfficiency = (wholeArmEfficiency + offsets) * postFactors;

                //Debug.Log(limbNames[i] + " Efficiency is " + wholeArmEfficiency);

                if (armEfficiency.Count > i)
                {
                    armEfficiency[i] = wholeArmEfficiency;
                }
                else
                {
                    armEfficiency.Add(wholeArmEfficiency);
                }
            }

            float baseEfficiency = 0;

            for (int i = 0; i < armEfficiency.Count; i++)
            {
                baseEfficiency += armEfficiency[i] / armEfficiency.Count;
            }
            
            manipulation = baseEfficiency;
        }
        else
        {
            manipulation = 0;
        }

        if (movingBP.Count > 0)
        {
            Dictionary<string, List<RWBodyPart>> limbs = new();
            List<string> limbNames = new();

            float offsets = 0;
            float postFactors = 1;

            for (int j = 0; j < movingBP.Count; j++)
            {
                string Temp = "";
                for (int i = 0; i < movingBP[j].name.Length; i++)
                {
                    char letter = movingBP[j].name[i];

                    if (letter.ToString() == " ")
                    {
                        if (limbs.TryGetValue(Temp, out _))
                        {
                            limbs[Temp].Add(movingBP[j]);
                        }
                        else
                        {
                            limbs[Temp] = new List<RWBodyPart>(1) { movingBP[j] };
                            limbNames.Add(Temp);

                            //Debug.Log("new LimbName " + Temp);
                        }

                        break;
                    }
                    else
                    {
                        Temp += letter;
                    }
                }
            }

            for (int i = 0; i < limbNames.Count; i++)
            {
                float leglimbEfficiency = 0;
                float tibiaEfficiency = 0;
                float femurEfficiency = 0;
                float footEfficiency = 0;
                float toeEfficiency = 0;

                float pelvisEfficiency = 0;
                float spineEfficiency = 0;

                int toeNumber = 0;

                for (int j = 0; j < limbs[limbNames[i]].Count; j++)
                {
                    if (limbs[limbNames[i]][j] is Leg)
                    {
                        leglimbEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Tibia)
                    {
                        tibiaEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Femur)
                    {
                        femurEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Foot)
                    {
                        footEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Toe)
                    {
                        toeEfficiency += limbs[limbNames[i]][j].efficiency;
                        toeNumber++;
                    }
                    else if (limbs[limbNames[i]][j] is Pelvis)
                    {
                        pelvisEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                    else if (limbs[limbNames[i]][j] is Spine)
                    {
                        spineEfficiency = limbs[limbNames[i]][j].efficiency;
                    }
                }

                float wholeLegEfficiency = (1 + (bloodPumping - 1f) * 0.2f) * (1f + (breathing - 1) * 0.2f) + (leglimbEfficiency) * (tibiaEfficiency) * (femurEfficiency) * (footEfficiency) * ((toeEfficiency * (0.4f / toeNumber)) + 0.6f) * pelvisEfficiency * spineEfficiency;

                wholeLegEfficiency *= Mathf.Min(1, consciousness);

                wholeLegEfficiency = (wholeLegEfficiency + offsets) * postFactors;

                //Debug.Log(limbNames[i] + " Efficiency is " + wholeLegEfficiency);

                if (legEfficiency.Count > i)
                {
                    legEfficiency[i] = wholeLegEfficiency;
                }
                else
                {
                    legEfficiency.Add(wholeLegEfficiency);
                }
            }

            float baseEfficiency = 0;

            for (int i = 0; i < legEfficiency.Count; i++)
            {
                baseEfficiency += legEfficiency[i] / legEfficiency.Count;
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
            float offsets = 0;
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
            float offsets = 0;
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

    public List<float> armEfficiency = new();
    public List<float> legEfficiency = new();

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