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

            for (int i = 0; i < fingers.Count; i++)
            {
                fingerEfficiency += fingers[i].efficiency;
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
        for (int i = 0; i < fingers.Count; i++)
        {
            if (fingers[i].health < fingers[i].maxHealth)
            {
                description += SetDescription(fingers[i]);
            }
        }

        return description;

        static string SetDescription(RWBodyPart part)
        {
            string name = part.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            return "  " + part.subName + " " + name + ": " + Mathf.Round(part.health) + " / " + part.maxHealth + "\n";
        }
    }

    public Shoulder shoulder;
    public Arm arm;
    public Hand hand;
    public List<Finger> fingers = new();

    public Clavicle clavicle;
    public Humerus humerus;
    public Radius radius;

    public float efficiency = 1;
}

public class LegSet
{
    public float Efficiency(RWState state, float offsets = 0, float postFactors = 1, float otherEfficiency = 1)
    {
        float toeEfficiency = 1;

        if (toes.Count != 0)
        {
            toeEfficiency = 0;

            for (int i = 0; i < toes.Count; i++)
            {
                toeEfficiency += toes[i].efficiency;
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
            description += SetDescription(leg);

            if (IsDestroyed(leg))
            {
                return description;
            }
        }
        if (femur != null && femur.health < femur.maxHealth)
        {
            description += SetDescription(femur);
        }
        if (tibia != null && tibia.health < tibia.maxHealth)
        {
            description += SetDescription(tibia);
        }

        if (foot != null && foot.health < foot.maxHealth)
        {
            if (IsDestroyed(foot))
            {
                return description;
            }
        }
        for (int i = 0; i < toes.Count; i++)
        {
            if (toes[i].health < toes[i].maxHealth)
            {
                description += SetDescription(toes[i]);
            }
        }

        return description;

        static string SetDescription(RWBodyPart part)
        {
            string name = part.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            return "  " + part.subName + " " + name + ": " + Mathf.Round(part.health) + " / " + part.maxHealth + "\n";
        }
    }

    public Leg leg;
    public Foot foot;
    public List<Toe> toes = new();

    public Femur femur;
    public Tibia tibia;

    public float efficiency = 1;
}