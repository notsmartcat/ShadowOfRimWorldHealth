using System.Collections.Generic;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

public class RWHealthState
{
    public static void NewRWHealthState(CreatureState self, RWState state)
    {
        state.bodySizeFactor = 1;

        List<RWBodyPart> bodyParts = new(33) {
                new UpperTorso(self),
                new LowerTorso(self),

                new Neck(self),
                new Head(self),
                new Eye(self),
                new Ear(self),
                new Nose(self),
                new Jaw(self),
                new Tongue(self),

                new Shoulder(self),
                new Arm(self),
                new Hand(self),
                new Finger(self),

                new Leg(self),
                new Foot(self),
                new Toe(self),

                new Tail(self),

                new Skull(self),

                new Spine(self),
                new Ribcage(self),
                new Sternum(self),

                new Clavicle(self),
                new Humerus(self),
                new Radius(self),

                new Pelvis(self),

                new Femur(self),
                new Tibia(self),

                new Brain(self),

                new Stomach(self),
                new Heart(self),
                new Lung(self),
                new Kidney(self),
                new Liver(self),
            };

        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (bodyParts[i].quantity == 2)
            {
                RWBodyPart bodyPart = null;
                if (bodyParts[i] is Eye)
                {
                    bodyPart = new Eye(self);
                }
                else if (bodyParts[i] is Ear)
                {
                    bodyPart = new Ear(self);
                }

                else if (bodyParts[i] is Shoulder)
                {
                    bodyPart = new Shoulder(self);

                    if (!state.armSetNames.Contains("Right"))
                    {
                        state.armSetNames.Add("Right");
                    }
                    if (!state.armSetNames.Contains("Left"))
                    {
                        state.armSetNames.Add("Left");
                    }

                    if (state.armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.shoulder = bodyParts[i] as Shoulder;
                    }
                    else
                    {
                        state.armSet.Add("Right", new ArmSet() { shoulder = bodyParts[i] as Shoulder });
                    }

                    if (state.armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.shoulder = bodyPart as Shoulder;
                    }
                    else
                    {
                        state.armSet.Add("Left", new ArmSet() { shoulder = bodyPart as Shoulder });
                    }
                }
                else if (bodyParts[i] is Arm)
                {
                    bodyPart = new Arm(self);

                    if (state.armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.arm = bodyParts[i] as Arm;
                    }
                    if (state.armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.arm = bodyPart as Arm;
                    }
                }
                else if (bodyParts[i] is Hand)
                {
                    bodyPart = new Hand(self);

                    if (state.armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.hand = bodyParts[i] as Hand;
                    }
                    if (state.armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.hand = bodyPart as Hand;
                    }
                }

                else if (bodyParts[i] is Leg)
                {
                    bodyPart = new Leg(self);

                    if (!state.legSetNames.Contains("Right"))
                    {
                        state.legSetNames.Add("Right");
                    }
                    if (!state.legSetNames.Contains("Left"))
                    {
                        state.legSetNames.Add("Left");
                    }

                    if (state.legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.leg = bodyParts[i] as Leg;
                    }
                    else
                    {
                        state.legSet.Add("Right", new LegSet() { leg = bodyParts[i] as Leg });
                    }

                    if (state.legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.leg = bodyPart as Leg;
                    }
                    else
                    {
                        state.legSet.Add("Left", new LegSet() { leg = bodyPart as Leg });
                    }
                }
                else if (bodyParts[i] is Foot)
                {
                    bodyPart = new Foot(self);

                    if (state.legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.foot = bodyParts[i] as Foot;
                    }
                    if (state.legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.foot = bodyPart as Foot;
                    }
                }

                else if (bodyParts[i] is Clavicle)
                {
                    bodyPart = new Clavicle(self);

                    if (state.armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.clavicle = bodyParts[i] as Clavicle;
                    }
                    if (state.armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.clavicle = bodyPart as Clavicle;
                    }
                }
                else if (bodyParts[i] is Humerus)
                {
                    bodyPart = new Humerus(self);

                    if (state.armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.humerus = bodyParts[i] as Humerus;
                    }
                    if (state.armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.humerus = bodyPart as Humerus;
                    }
                }
                else if (bodyParts[i] is Radius)
                {
                    bodyPart = new Radius(self);

                    if (state.armSet.TryGetValue("Right", out ArmSet rightSet))
                    {
                        rightSet.radius = bodyParts[i] as Radius;
                    }
                    if (state.armSet.TryGetValue("Left", out ArmSet leftSet))
                    {
                        leftSet.radius = bodyPart as Radius;
                    }
                }

                else if (bodyParts[i] is Femur)
                {
                    bodyPart = new Femur(self);

                    if (state.legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.femur = bodyParts[i] as Femur;
                    }
                    if (state.legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.femur = bodyPart as Femur;
                    }
                }
                else if (bodyParts[i] is Tibia)
                {
                    bodyPart = new Tibia(self);

                    if (state.legSet.TryGetValue("Right", out LegSet rightSet))
                    {
                        rightSet.tibia = bodyParts[i] as Tibia;
                    }
                    if (state.legSet.TryGetValue("Left", out LegSet leftSet))
                    {
                        leftSet.tibia = bodyPart as Tibia;
                    }
                }

                else if (bodyParts[i] is Lung)
                {
                    bodyPart = new Lung(self);
                }
                else if (bodyParts[i] is Kidney)
                {
                    bodyPart = new Kidney(self);
                }

                if (bodyPart != null)
                {
                    bodyPart.subName = "Left";

                    state.bodyParts.Add(bodyPart);
                }

                bodyParts[i].subName = "Right";
                state.bodyParts.Add(bodyParts[i]);
            }
            else if (bodyParts[i] is Finger finger)
            {
                for (int k = 0; k < state.armSetNames.Count; k++)
                {
                    for (int j = 0; j < bodyParts[i].quantity / state.armSetNames.Count; j++)
                    {
                        finger ??= new Finger(self);

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

                        finger.subName = state.armSetNames[k];
                        finger.group.Add(state.armSetNames[k] + "Hand");

                        if (state.armSet.TryGetValue(state.armSetNames[k], out ArmSet set))
                        {
                            set.fingers.Add(finger);
                        }

                        state.bodyParts.Add(finger);

                        finger = null;
                    }
                }
            }
            else if (bodyParts[i] is Toe toe)
            {
                for (int k = 0; k < state.legSetNames.Count; k++)
                {
                    for (int j = 0; j < bodyParts[i].quantity / state.legSetNames.Count; j++)
                    {
                        toe ??= new Toe(self);

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

                        toe.subName = state.legSetNames[k];

                        if (state.legSet.TryGetValue(state.legSetNames[k], out LegSet set))
                        {
                            set.toes.Add(toe);
                        }

                        state.bodyParts.Add(toe);

                        toe = null;
                    }
                }
            }
            else
            {
                state.bodyParts.Add(bodyParts[i]);

                if (bodyParts[i] is Brain brain)
                {
                    state.consciousnessSource = brain;
                }
            }
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            state.maxHealth += state.bodyParts[i].maxHealth;

            if (state.bodyParts[i].capacity.Count == 0 || state.bodyParts[i] is Shoulder || state.bodyParts[i] is Arm || state.bodyParts[i] is Hand || state.bodyParts[i] is Finger || state.bodyParts[i] is Leg || state.bodyParts[i] is Foot || state.bodyParts[i] is Clavicle || state.bodyParts[i] is Humerus || state.bodyParts[i] is Radius || state.bodyParts[i] is Femur || state.bodyParts[i] is Tibia || state.bodyParts[i] is Toe)
            {
                continue;
            }

            if (state.bodyParts[i].capacity.Contains("Blood Filtration"))
            {
                state.bloodFiltrationBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Blood Pumping"))
            {
                state.bloodPumpingBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Breathing"))
            {
                state.breathingBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Digestion"))
            {
                state.digestionBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Eating"))
            {
                state.eatingBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Hearing"))
            {
                state.hearingBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Manipulation"))
            {
                state.manipulationBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Moving"))
            {
                state.movingBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Sight"))
            {
                state.sightBP.Add(state.bodyParts[i]);
            }
            if (state.bodyParts[i].capacity.Contains("Talking"))
            {
                state.talkingBP.Add(state.bodyParts[i]);
            }
        }
    }

    public static void Update(CreatureState self, RWState state)
    {
        if (self.dead)
        {
            goto dead;
        }

        if (state.healingRate > 0)
        {
            state.healingRate--;
        }

        bool heal = state.healingRate <= 0;

        if (heal)
        {
            state.healingRate = state.healingRateTics;
        }

        List<RWInjury> healList = new();

        List<RWAffliction> afflictionList;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i].afflictions.Count == 0 || IsSubPartDestroyed(state, state.bodyParts[i]))
            {
                continue;
            }

            afflictionList = new(state.bodyParts[i].afflictions);

            for (int j = 0; j < afflictionList.Count; j++)
            {
                if (afflictionList[j] is not RWInjury injury)
                {
                    if (afflictionList[j] is RWDisease disease)
                    {
                        Disease(disease, state.bodyParts[i]);
                    }

                    continue;
                }

                if (injury.infectionTimer > 0)
                {
                    injury.infectionTimer--;

                    if (injury.infectionTimer <= 0)
                    {
                        float infectionChance = injury.healingDifficulty.infectionChance * (injury.isTended ? Mathf.Lerp(0.85f, 0.05f , injury.tendQuality) : 1);

                        if (self.creature.Room != null && self.creature.Room.shelter)
                        {
                            infectionChance *= 0.5f;
                        }

                        float random = Random.value;

                        if (random < infectionChance)
                        {
                            state.bodyParts[i].afflictions.Add(new RWInfection(self, state.bodyParts[i]));
                        }
                    }
                }

                if (heal && injury is not RWDestroyed && (injury is not RWScar scar || !scar.isRevealed))
                {
                    healList.Add(injury);
                }
            }
        }

        afflictionList = new(state.wholeBodyAfflictions);

        for (int i = 0; i < afflictionList.Count; i++)
        {
            if (afflictionList[i] is RWDisease disease)
            {
                Disease(disease);
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

            if (injury is RWScar scar)
            {
                if (scar.damage <= scar.scarDamage)
                {
                    scar.damage = scar.scarDamage;
                    scar.isTended = true;
                    scar.isBleeding = false;
                    scar.isRevealed = true;
                }
            }
            else if (injury.damage <= 0)
            {
                injury.healingDifficulty = null;
                injury.part = null;
                injury.part.afflictions.Remove(injury);
            }

            state.updateCapacities = true;
        }

        float prevbloodLoss = state.bloodLoss;

        if (state.bloodLossPerCycle == 0 && state.bloodLoss > 0)
        {
            state.bloodLoss -= 0.333f / (40 * 60 * state.cycleLength);

            if (updateBloodLoss())
            {
                state.updateCapacities = true;
            }
        } //Replenishes 33.3% of blood per cycle if not bleeding
        else if (state.bloodLossPerCycle > 0)
        {
            state.bloodLoss += state.bloodLossPerCycle / 100 / (40 * 60 * state.cycleLength);

            if (updateBloodLoss())
            {
                state.updateCapacities = true;
            }
        }

        state.bloodLoss = Mathf.Clamp(state.bloodLoss, 0, 1);

    dead:

        if (state.updateCapacities)
        {
            state.updateCapacities = false;
            UpdateCapacities();
        }

        void Disease(RWDisease disease, RWBodyPart part = null)
        {
            if (disease.severity >= 1)
            {
                return;
            }

            float prevSeverity = disease.severity;

            if (!disease.isImmune)
            {
                disease.severity += disease.severityGain * disease.InfectionLuck / (40 * 60 * state.cycleLength);

                if (disease.isTended)
                {
                    disease.severity -= disease.treatment * disease.tendQuality / (40 * 60 * state.cycleLength);
                }

                disease.immunity += disease.immunityGain / (40 * 60 * state.cycleLength);

                disease.timeUntilTreatment -= 1 / (40 * 60 * state.cycleLength);

                if (disease.timeUntilTreatment <= -3)
                {
                    disease.isTended = false;
                }
            }
            else
            {
                disease.severity -= disease.severityLoss / (40 * 60 * state.cycleLength);
            }

            if (disease.isImmune && disease.severity <= 0)
            {
                if (part == null)
                {
                    state.wholeBodyAfflictions.Remove(disease);
                    state.updateCapacities = true;
                }
                else
                {
                    disease.part = null;
                    part.afflictions.Remove(disease);
                    state.updateCapacities = true;
                }

                return;
            }
            else if (prevSeverity == disease.severity)
            {
                return;
            }

            if (disease is RWFlu && (disease.severity <= 0.665f && prevSeverity > 0.665f || disease.severity > 0.665f && prevSeverity <= 0.665f || disease.severity <= 0.832f && prevSeverity > 0.832f || disease.severity > 0.832f && prevSeverity <= 0.832f))
            {
                state.updateCapacities = true;
            }
            else if (disease is RWInfection && (disease.severity <= 0.32f && prevSeverity > 0.32f || disease.severity > 0.32f && prevSeverity <= 0.32f || disease.severity <= 0.77f && prevSeverity > 0.77f || disease.severity > 0.77f && prevSeverity <= 0.77f || disease.severity <= 0.86f && prevSeverity > 0.86f || disease.severity > 0.86f && prevSeverity <= 0.86f))
            {
                state.updateCapacities = true;
            }
        }

        void UpdateCapacities()
        {
            Debug.Log("Capacities Update");

            state.capacityAffectingAffliction.Clear();

            if (!self.dead)
            {
                state.bloodLossPerCycle = 0;

                state.pain = 0;

                state.consciousness = 0;
                state.moving = 0;
                state.manipulation = 0;
                state.talking = 0;
                state.eating = 0;
                state.sight = 0;
                state.hearing = 0;
                state.breathing = 0;
                state.bloodFiltration = 0;
                state.bloodPumping = 0;
                state.digestion = 0;
            }

            List<RWAffliction> afflictionList;

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                state.bodyParts[i].health = state.bodyParts[i].maxHealth;

                if (state.bodyParts[i].afflictions.Count == 0)
                {
                    goto line1;
                }
                else if (IsSubPartDestroyed(state, state.bodyParts[i]))
                {
                    state.bodyParts[i].health = 0;

                    state.bodyParts[i].efficiency = 0;

                    goto line1;
                }

                afflictionList = new(state.bodyParts[i].afflictions);

                for (int j = 0; j < afflictionList.Count; j++)
                {
                    if (afflictionList[j] is RWDestroyed destroyed)
                    {
                        state.bodyParts[i].health = 0;

                        state.bodyParts[i].efficiency = 0;

                        state.bloodLossPerCycle += destroyed.isBleeding && !destroyed.isTended ? destroyed.healingDifficulty.bleeding * state.bodySizeFactor * BloodLossMultiplier(state.bodyParts[i]) : 0;

                        break;
                    }

                    if (afflictionList[j] is not RWInjury injury)
                    {
                        if (!self.dead && afflictionList[j] is RWDisease disease)
                        {
                            Disease(disease);
                        }

                        continue;
                    }

                    state.bodyParts[i].health -= injury.damage;

                    if (self.dead)
                    {
                        continue;
                    }

                    if (injury is RWScar scar && scar.isRevealed)
                    {
                        if (scar.painCategory == "painful")
                        {
                            afflictionList[j].pain = scar.scarDamage * 1.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                        else if (scar.painCategory == "aching")
                        {
                            afflictionList[j].pain = scar.scarDamage * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                        else if (scar.painCategory == "itchy")
                        {
                            afflictionList[j].pain = scar.scarDamage * 0.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                    }
                    else
                    {
                        afflictionList[j].pain = injury.damage * injury.healingDifficulty.pain / state.bodySizeFactor / 100;
                    }

                    state.pain += afflictionList[j].pain;

                    state.bloodLossPerCycle += injury.isBleeding && !injury.isTended ? injury.healingDifficulty.bleeding * injury.damage * state.bodySizeFactor * BloodLossMultiplier(state.bodyParts[i]) : 0;
                }

            line1:

                state.bodyParts[i].efficiency = Mathf.Max(0, state.bodyParts[i].health / state.bodyParts[i].maxHealth);

                if ((state.bodyParts[i].health > 0 && state.bodyParts[i].health < 1) || state.bodyParts[i].deathEffect == "" && state.bodyParts[i].health < 1)
                {
                    state.bodyParts[i].health = 1;
                    state.bodyParts[i].efficiency = 0;
                }
                else if (state.bodyParts[i] is UpperTorso && state.bodyParts[i].health <= 0)
                {
                    state.bodyParts[i].health = 0;
                    state.bodyParts[i].efficiency = 0;
                }

                if (self.dead)
                {
                    continue;
                }
            }

            if (self.dead)
            {
                return;
            }

            afflictionList = new(state.wholeBodyAfflictions);

            for (int i = 0; i < afflictionList.Count; i++)
            {
                if (afflictionList[i] is RWDisease disease)
                {
                    Disease(disease);
                }
            }

            state.pain = Mathf.Clamp(state.pain, 0, 1);

            if (state.bloodFiltrationBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.bloodFiltration;
                float postFactors = 1;

                for (int i = 0; i < state.bloodFiltrationBP.Count; i++)
                {
                    baseEfficiency += (state.bloodFiltrationBP[i] is Kidney ? (state.bloodFiltrationBP[i].efficiency / 2) : state.bloodFiltrationBP[i].efficiency) / (state.bloodFiltrationBP.Count != 1 ? state.bloodFiltrationBP.Count - 1 : state.bloodFiltrationBP.Count);
                }

                state.bloodFiltration = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.bloodFiltration = 0;
            }

            if (state.bloodPumpingBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.bloodPumping;
                float postFactors = 1;

                for (int i = 0; i < state.bloodPumpingBP.Count; i++)
                {
                    baseEfficiency += state.bloodPumpingBP[i].efficiency / state.bloodPumpingBP.Count;
                }

                state.bloodPumping = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.bloodPumping = 0;
            }

            if (state.breathingBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.breathing;
                float postFactors = 1;

                for (int i = 0; i < state.breathingBP.Count; i++)
                {
                    baseEfficiency += (state.breathingBP[i] is Lung ? (state.breathingBP[i].efficiency / 2) : state.breathingBP[i].efficiency) / (state.breathingBP.Count != 1 ? (state.breathingBP.Count - 1) : state.breathingBP.Count);
                }

                state.breathing = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.breathing = 0;
            }

            float consciounessOffset = state.consciousness;

            state.consciousness = ((state.consciousnessSource == null ? 1 : state.consciousnessSource.efficiency) * (1 - Mathf.Clamp((state.pain - 0.1f) * 4 / 9, 0, 0.4f)) * (1 - 0.2f * (1 - state.bloodPumping)) * (1 - 0.2f * (1 - state.breathing)) * (1 - 0.1f * (1 - state.bloodFiltration))) + consciounessOffset;

            if (state.bloodLoss >= 0.6f)
            {
                state.consciousness -= 0.4f;

                state.forceUnconsciousness = true;
            }
            else if (state.bloodLoss >= 0.45f)
            {
                state.consciousness -= 0.4f;
            }
            else if (state.bloodLoss >= 0.3f)
            {
                state.consciousness -= 0.2f;
            }
            else if (state.bloodLoss >= 0.15f)
            {
                state.consciousness -= 0.1f;
            }

            if (state.forceUnconsciousness)
            {
                state.consciousness = Mathf.Min(state.consciousness, 0.1f);
            }

            state.consciousness = Mathf.Max(state.consciousness, 0);

            if (state.digestionBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.digestion;
                float postFactors = 1;

                for (int i = 0; i < state.digestionBP.Count; i++)
                {
                    baseEfficiency += ((state.digestionBP[i] is Stomach || state.digestionBP[i] is Liver) ? state.digestionBP[i].efficiency / 2 : state.digestionBP[i].efficiency) / (state.digestionBP.Count != 1 ? state.digestionBP.Count - 1 : state.digestionBP.Count);
                }

                state.digestion = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.digestion = 0;
            }

            if (state.eatingBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.eating;
                float postFactors = 1;

                for (int i = 0; i < state.eatingBP.Count; i++)
                {
                    baseEfficiency += state.eatingBP[i].efficiency / state.eatingBP.Count;
                }

                state.eating = Mathf.Max(0.1f, ((baseEfficiency * state.consciousness) + offsets) * postFactors);
            }
            else
            {
                state.eating = 0;
            }

            if (state.hearingBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.hearing;
                float postFactors = 1;

                if (state.hearingBP.Count == 1)
                {
                    baseEfficiency = state.hearingBP[0].efficiency;

                    state.hearing = (baseEfficiency + offsets) * postFactors;
                }
                else
                {
                    float bestEfficiency = 0;

                    for (int i = 0; i < state.hearingBP.Count; i++)
                    {
                        baseEfficiency += state.hearingBP[i].efficiency / (state.hearingBP.Count * 2);

                        if (state.hearingBP[i].efficiency > bestEfficiency)
                        {
                            bestEfficiency = state.hearingBP[i].efficiency;
                        }
                    }

                    baseEfficiency += bestEfficiency / 2;

                    state.hearing = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
                }
            }
            else
            {
                state.hearing = 0;
            }

            if ((state.armSetNames.Count + state.manipulationBP.Count) > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.manipulation;
                float postFactors = 1;
                float otherEfficiency = 1;

                for (int i = 0; i < state.manipulationBP.Count; i++)
                {
                    otherEfficiency *= state.manipulationBP[i].efficiency;
                }

                for (int i = 0; i < state.armSetNames.Count; i++)
                {
                    baseEfficiency += state.armSet[state.armSetNames[i]].Efficiency(state, offsets / state.armSetNames.Count, postFactors, otherEfficiency) / state.armSetNames.Count;
                }

                state.manipulation = Mathf.Max(0, baseEfficiency);
            }
            else
            {
                state.manipulation = 0;
            }

            if ((state.legSetNames.Count + state.movingBP.Count) > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.moving;
                float postFactors = 1;
                float otherEfficiency = 1;

                for (int i = 0; i < state.movingBP.Count; i++)
                {
                    otherEfficiency *= state.movingBP[i].efficiency;
                }

                for (int i = 0; i < state.legSetNames.Count; i++)
                {
                    baseEfficiency += state.legSet[state.legSetNames[i]].Efficiency(state, offsets / state.armSetNames.Count, postFactors, otherEfficiency) / state.legSetNames.Count;
                }

                state.moving = state.moving = Mathf.Max(0, baseEfficiency);
            }
            else
            {
                state.moving = 0;
            }

            if (state.sightBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.sight;
                float postFactors = 1;

                if (state.sightBP.Count == 1)
                {
                    baseEfficiency = state.sightBP[0].efficiency;

                    state.sight = (baseEfficiency + offsets) * postFactors;
                }
                else
                {
                    float bestEfficiency = 0;

                    for (int i = 0; i < state.sightBP.Count; i++)
                    {
                        baseEfficiency += state.sightBP[i].efficiency / (state.sightBP.Count * 2);

                        if (state.sightBP[i].efficiency > bestEfficiency)
                        {
                            bestEfficiency = state.sightBP[i].efficiency;
                        }
                    }

                    baseEfficiency += bestEfficiency / 2;

                    state.sight = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
                }
            }
            else
            {
                state.sight = 0;
            }

            if (state.talkingBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.talking;
                float postFactors = 1;

                for (int i = 0; i < state.talkingBP.Count; i++)
                {
                    baseEfficiency += state.talkingBP[i].efficiency / state.talkingBP.Count;
                }

                state.talking = Mathf.Max(0, ((baseEfficiency * state.consciousness) + offsets) * postFactors);
            }
            else
            {
                state.talking = 0;
            }

            void Disease(RWDisease disease)
            {
                if (disease is RWFlu)
                {
                    state.capacityAffectingAffliction.Add(disease);

                    if (disease.severity <= 0.665f)
                    {
                        state.consciousness -= 0.05f;
                        state.manipulation -= 0.05f;
                        state.breathing -= 0.1f;
                    }
                    else if (disease.severity <= 0.832f)
                    {
                        state.consciousness -= 0.1f;
                        state.manipulation -= 0.1f;
                        state.breathing -= 0.15f;
                    }
                    else
                    {
                        state.pain += 0.05f;

                        state.consciousness -= 0.15f;
                        state.manipulation -= 0.2f;
                        state.breathing -= 0.2f;
                    }
                }
                else if (disease is RWInfection)
                {
                    if (disease.severity <= 0.32f)
                    {
                        state.pain += 0.05f;
                    }
                    else if (disease.severity <= 0.77f)
                    {
                        state.pain += 0.08f;
                    }
                    else if (disease.severity <= 0.86f)
                    {
                        state.pain += 0.12f;

                        state.consciousness -= 0.5f;

                        state.capacityAffectingAffliction.Add(disease);
                    }
                    else
                    {
                        state.forceUnconsciousness = true;

                        state.pain += 0.85f;

                        state.breathing -= 0.5f;

                        state.capacityAffectingAffliction.Add(disease);
                    }
                }
            }
        }

        bool updateBloodLoss()
        {
            return state.bloodLoss >= 0.6f && prevbloodLoss < 0.6f || state.bloodLoss < 0.6f && prevbloodLoss >= 0.6f || state.bloodLoss >= 0.45f && prevbloodLoss < 0.45f || state.bloodLoss < 0.45f && prevbloodLoss >= 0.45f || state.bloodLoss >= 0.3f && prevbloodLoss < 0.3f || state.bloodLoss < 0.3f && prevbloodLoss >= 0.3f || state.bloodLoss >= 0.15f && prevbloodLoss < 0.15f || state.bloodLoss < 0.15f && prevbloodLoss >= 0.15f;
        }
    }

    public static void Damage(CreatureState self, RWState state, RWDamageType damageType, float damage, RWBodyPart bodyPart, string attackName = "", string attackerName = "")
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

        state.updateCapacities = true;

        if (damageType.armourCategory == "Blunt")
        {
            damage = 0;
        }

        while (extraDamage > 0 || damage > 0)
        {
            bool bodypartHit = false;

            if (focusedBodyPart.isInternal && focusedBodyPart.subPartOf != "")
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (IsSubPartName(focusedBodyPart, state.bodyParts[i]))
                    {
                        focusedBodyPart = state.bodyParts[i];

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

                        state.updateCapacities = true;

                        bodypartHit = true;

                        break;
                    }
                }
            }
            else if (extraDamage > 0 && damageType is RWBomb && BombDestroyBodyparts())
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (IsSubPartName(focusedBodyPart, state.bodyParts[i]))
                    {
                        focusedBodyPart = state.bodyParts[i];

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

                        state.updateCapacities = true;

                        bodypartHit = true;

                        break;
                    }
                }
            }
            else if (extraDamage > 0 && damageType is RWSuperBomb)
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (IsSubPartName(focusedBodyPart, state.bodyParts[i]))
                    {
                        focusedBodyPart = state.bodyParts[i];

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

                        state.updateCapacities = true;

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

                    for (int i = 0; i < state.bodyParts.Count; i++)
                    {
                        for (int j = 0; j < subParts.Count; j++)
                        {
                            if (!subParts.Contains(state.bodyParts[i]) && !subPartsRestricted.Contains(state.bodyParts[i]) && IsSubPartName(state.bodyParts[i], subParts[j]))
                            {
                                if (subParts[j].afflictions.Count == 1 && subParts[j].afflictions[0] is RWDestroyed || subParts[j].deathEffect == "")
                                {
                                    subPartsRestricted.Add(state.bodyParts[i]);
                                    continue;
                                }

                                newBodyParts = true;
                                subParts.Add(state.bodyParts[i]);
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
                    subParts[j].afflictions.Add(new RWDestroyed(self, subParts[j], 0f, damageType, attackName, attackerName));
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
                return new(self, focusedBodyPart, damage, damageType, attackName, attackerName);
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
            else
            {
                oddsOfScarring = (focusedBodyPart.isSolid ? 1f : 2f) * Mathf.Clamp((damage - 4) / 10, 0, 1);
            }

            if ((Random.value * 100) >= oddsOfScarring)
            {
                return new(self, focusedBodyPart, damage, damageType, attackName, attackerName);
            }

            RWScar scar = new(self, focusedBodyPart, damage, damageType, attackName, attackerName);

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

    public static int BloodLossMultiplier(RWBodyPart part)
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