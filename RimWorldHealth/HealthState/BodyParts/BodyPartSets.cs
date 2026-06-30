using System.Collections.Generic;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

public class ArmSet
{
    public float Efficiency(RWState state, float offsets = 0, float postFactors = 1, float otherEfficiency = 1)
    {
        float fingerEfficiency = 1;

        if (fingers.Count != 0)
        {
            fingerEfficiency = 0;

            foreach (Finger finger in fingers)
            {
                fingerEfficiency += finger.efficiency;
            }

            fingerEfficiency = (fingerEfficiency * (0.8f / fingers.Count)) + 0.2f;
        }

        efficiency = (arm != null ? arm.efficiency : 1) * (shoulder != null ? shoulder.efficiency : 1) * (clavicle != null ? clavicle.efficiency : 1) * (humerus != null ? humerus.efficiency : 1) * (radius != null ? radius.efficiency : 1) * (hand != null ? hand.efficiency : 1) * fingerEfficiency * otherEfficiency;

        efficiency = ((state.consciousness * efficiency) + offsets) * postFactors;

        return efficiency;
    }

    public string CapacityAffectingAffliction()
    {
        string description = "";

        if (shoulder != null && shoulder.health < shoulder.maxHealth)
        {
            description += SetDescription(shoulder);

            if (IsDestroyed(shoulder))
            {
                return description;
            }
        }
        if (clavicle != null && clavicle.health < clavicle.maxHealth)
        {
            description += SetDescription(clavicle);
        }

        if (arm != null && arm.health < arm.maxHealth)
        {
            description += SetDescription(arm);

            if (IsDestroyed(arm))
            {
                return description;
            }
        }
        if (humerus != null && humerus.health < humerus.maxHealth)
        {
            description += SetDescription(humerus);
        }
        if (radius != null && radius.health < radius.maxHealth)
        {
            description += SetDescription(radius);
        }

        if (hand != null && hand.health < hand.maxHealth)
        {
            description += SetDescription(hand);

            if (IsDestroyed(hand))
            {
                return description;
            }
        }
        foreach (Finger finger in fingers)
        {
            if (finger.health < finger.maxHealth)
            {
                description += SetDescription(finger);
            }
        }

        return description;
    }

    public Dictionary<string, List<RWBodyPart>> KarmaFlowerGetMainMissingPart(string key, Dictionary<string, List<RWBodyPart>> dic)
    {
        List<RWBodyPart> list = new();

        if (shoulder != null && shoulder.afflictions.Count == 1 && shoulder.afflictions[0] is RWDestroyed)
        {
            list.Add(shoulder);
            dic.Add(key, list);
            return dic;
        }
        else if (clavicle != null && clavicle.afflictions.Count == 1 && clavicle.afflictions[0] is RWDestroyed)
        {
            list.Add(clavicle);
            dic.Add(key, list);
            return dic;
        }
        else if (arm != null && arm.afflictions.Count == 1 && arm.afflictions[0] is RWDestroyed)
        {
            list.Add(arm);
            dic.Add(key, list);
            return dic;
        }

        if (humerus != null && humerus.afflictions.Count == 1 && humerus.afflictions[0] is RWDestroyed)
        {
            list.Add(humerus);
        }
        if (radius != null && radius.afflictions.Count == 1 && radius.afflictions[0] is RWDestroyed)
        {
            list.Add(radius);
        }
        if (hand != null && hand.afflictions.Count == 1 && hand.afflictions[0] is RWDestroyed)
        {
            list.Add(hand);
        }

        if (list.Count > 0)
        {
            dic.Add(key, list);
        }

        return dic;
    }

    public void KarmaFlowerHeal(RWState state, RWBodyPart part)
    {
        if (part == shoulder)
        {
            shoulder.afflictions.Clear();
            arm.afflictions.Clear();
            hand.afflictions.Clear();
            foreach (Finger finger in fingers)
            {
                finger.afflictions.Clear();
            }
            clavicle.afflictions.Clear();
            humerus.afflictions.Clear();
            radius.afflictions.Clear();
        }
        else if (part == arm)
        {
            arm.afflictions.Clear();
            hand.afflictions.Clear();
            foreach (Finger finger in fingers)
            {
                finger.afflictions.Clear();
            }
            humerus.afflictions.Clear();
            radius.afflictions.Clear();
        }
        else if (part == hand)
        {
            hand.afflictions.Clear();
            foreach (Finger finger in fingers)
            {
                finger.afflictions.Clear();
            }
        }
        else
        {
            part.afflictions.Clear();
        }

        state.updateCapacities = true;
    }

    public Shoulder shoulder;
    public Arm arm;
    public Hand hand;
    public List<Finger> fingers = new();

    public Clavicle clavicle;
    public Humerus humerus;
    public Radius radius;

    public float efficiency = 1;

    public static string SetDescription(RWBodyPart part)
    {
        string name = part.name;
        name = char.ToLowerInvariant(name[0]) + name.Substring(1);

        return "  " + part.subName + " " + name + ": " + Mathf.Round(part.health) + " / " + part.maxHealth + "\n";
    }
}

public class LegSet
{
    public float Efficiency(RWState state, float offsets = 0, float postFactors = 1, float otherEfficiency = 1)
    {
        float toeEfficiency = 1;

        if (toes.Count != 0)
        {
            toeEfficiency = 0;

            foreach (Toe toe in toes)
            {
                toeEfficiency += toe.efficiency;
            }

            toeEfficiency = (toeEfficiency * (0.4f / toes.Count)) + 0.6f;
        }

        efficiency = (leg != null ? leg.efficiency : 1) * (tibia != null ? tibia.efficiency : 1) * (femur != null ? femur.efficiency : 1) * (foot != null ? foot.efficiency : 1) * toeEfficiency * otherEfficiency;

        efficiency = ((Mathf.Min(1, state.consciousness) * (1 + (state.bloodPumping - 1f) * 0.2f) * (1f + (state.breathing - 1) * 0.2f) * efficiency) + offsets) * postFactors;

        return efficiency;
    }

    public string CapacityAffectingAffliction()
    {
        string description = "";

        if (leg != null && leg.health < leg.maxHealth)
        {
            description += ArmSet.SetDescription(leg);

            if (IsDestroyed(leg))
            {
                return description;
            }
        }
        if (femur != null && femur.health < femur.maxHealth)
        {
            description += ArmSet.SetDescription(femur);
        }
        if (tibia != null && tibia.health < tibia.maxHealth)
        {
            description += ArmSet.SetDescription(tibia);
        }

        if (foot != null && foot.health < foot.maxHealth)
        {
            if (IsDestroyed(foot))
            {
                return description;
            }
        }
        foreach (Toe toe in toes)
        {
            if (toe.health < toe.maxHealth)
            {
                description += ArmSet.SetDescription(toe);
            }
        }

        return description;
    }

    public Dictionary<string, List<RWBodyPart>> KarmaFlowerGetMainMissingPart(string key, Dictionary<string, List<RWBodyPart>> dic)
    {
        List<RWBodyPart> list = new();

        if (leg != null && leg.afflictions.Count == 1 && leg.afflictions[0] is RWDestroyed)
        {
            list.Add(leg);
            dic.Add(key, list);
            return dic;
        }

        if (femur != null && femur.afflictions.Count == 1 && femur.afflictions[0] is RWDestroyed)
        {
            list.Add(femur);
        }
        if (tibia != null && tibia.afflictions.Count == 1 && tibia.afflictions[0] is RWDestroyed)
        {
            list.Add(tibia);
        }
        if (foot != null && foot.afflictions.Count == 1 && foot.afflictions[0] is RWDestroyed)
        {
            list.Add(foot);
        }

        if (list.Count > 0)
        {
            dic.Add(key, list);
        }

        return dic;
    }

    public void KarmaFlowerHeal(RWState state, RWBodyPart part)
    {
        if (part == leg)
        {
            leg.afflictions.Clear();
            foot.afflictions.Clear();
            foreach (Toe toe in toes)
            {
                toe.afflictions.Clear();
            }
            femur.afflictions.Clear();
            tibia.afflictions.Clear();
        }
        else if (part == foot)
        {
            foot.afflictions.Clear();
            foreach (Toe toe in toes)
            {
                toe.afflictions.Clear();
            }
        }
        else
        {
            part.afflictions.Clear();
        }

        state.updateCapacities = true;
    }

    public Leg leg;
    public Foot foot;
    public List<Toe> toes = new();

    public Femur femur;
    public Tibia tibia;

    public float efficiency = 1;
}