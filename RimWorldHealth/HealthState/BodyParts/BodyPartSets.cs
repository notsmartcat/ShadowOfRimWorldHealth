using System.Collections.Generic;
using UnityEngine;

namespace ShadowOfRimWorldHealth;

public class ArmSet
{
    public float Efficiency(RWPlayerHealthState state, float offsets = 0, float postFactors = 1, float otherEfficiency = 1)
    {
        float fingerEfficiency = 0;

        for (int i = 0; i < fingers.Count; i++)
        {
            fingerEfficiency += fingers[i].efficiency;
        }

        efficiency = (arm != null ? arm.efficiency : 0) * (shoulder != null ? shoulder.efficiency : 0) * (clavicle != null ? clavicle.efficiency : 0) * (humerus != null ? humerus.efficiency : 0) * (radius != null ? radius.efficiency : 0) * (hand != null ? hand.efficiency : 0) * ((fingerEfficiency * (0.8f / fingers.Count)) + 0.2f) * otherEfficiency;

        efficiency = ((state.consciousness * efficiency) + offsets) * postFactors;

        return efficiency;
    }

    public string CapacityAffectingAffliction()
    {
        string description = "";

        string name;

        if (shoulder != null && shoulder.efficiency < 1)
        {
            name = shoulder.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + shoulder.subName + " " + name + ": " + Mathf.Floor(shoulder.health) + " / " + shoulder.maxHealth + "\n";
        }

        if (clavicle != null && clavicle.efficiency < 1)
        {
            name = clavicle.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + clavicle.subName + " " + name + ": " + Mathf.Floor(clavicle.health) + " / " + clavicle.maxHealth + "\n";
        }

        if (arm != null && arm.efficiency < 1)
        {
            name = arm.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + arm.subName + " " + name + ": " + Mathf.Floor(arm.health) + " / " + arm.maxHealth + "\n";
        }

        if (humerus != null && humerus.efficiency < 1)
        {
            name = humerus.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + humerus.subName + " " + name + ": " + Mathf.Floor(humerus.health) + " / " + humerus.maxHealth + "\n";
        }

        if (hand != null && hand.efficiency < 1)
        {
            name = hand.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + hand.subName + " " + name + ": " + Mathf.Floor(hand.health) + " / " + hand.maxHealth + "\n";
        }

        if (radius != null && radius.efficiency < 1)
        {
            name = radius.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + radius.subName + " " + name + ": " + Mathf.Floor(radius.health) + " / " + radius.maxHealth + "\n";
        }

        for (int i = 0; i < fingers.Count; i++)
        {
            if (fingers[i].efficiency < 1)
            {
                name = fingers[i].name;
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);

                description += "  " + fingers[i].subName + " " + name + ": " + Mathf.Floor(fingers[i].health) + " / " + fingers[i].maxHealth + "\n";
            }
        }

        return description;
    }

    public Shoulder shoulder;
    public Arm arm;
    public Hand hand;
    public List<Finger> fingers = new();

    public Clavicle clavicle;
    public Humerus humerus;
    public Radius radius;

    public float efficiency = 0;
}

public class LegSet
{
    public float Efficiency(RWPlayerHealthState state, float offsets = 0, float postFactors = 1, float otherEfficiency = 1)
    {
        float toeEfficiency = 0;

        for (int i = 0; i < toes.Count; i++)
        {
            toeEfficiency += toes[i].efficiency;
        }

        efficiency = (leg != null ? leg.efficiency : 0) * (tibia != null ? tibia.efficiency : 0) * (femur != null ? femur.efficiency : 0) * (foot != null ? foot.efficiency : 0) * ((toeEfficiency * (0.4f / toes.Count)) + 0.6f) * otherEfficiency;

        efficiency = ((Mathf.Min(1, state.consciousness) * (1 + (state.bloodPumping - 1f) * 0.2f) * (1f + (state.breathing - 1) * 0.2f) * efficiency) + offsets) * postFactors;

        return efficiency;
    }

    public string CapacityAffectingAffliction()
    {
        string description = "";

        string name;

        if (leg != null && leg.efficiency < 1)
        {
            name = leg.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + leg.subName + " " + name + ": " + Mathf.Floor(leg.health) + " / " + leg.maxHealth + "\n";
        }

        if (femur != null && femur.efficiency < 1)
        {
            name = femur.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + femur.subName + " " + name + ": " + Mathf.Floor(femur.health) + " / " + femur.maxHealth + "\n";
        }

        if (foot != null && foot.efficiency < 1)
        {
            name = foot.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + foot.subName + " " + name + ": " + Mathf.Floor(foot.health) + " / " + foot.maxHealth + "\n";
        }

        if (tibia != null && tibia.efficiency < 1)
        {
            name = tibia.name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            description += "  " + tibia.subName + " " + name + ": " + Mathf.Floor(tibia.health) + " / " + tibia.maxHealth + "\n";
        }

        for (int i = 0; i < toes.Count; i++)
        {
            if (toes[i].efficiency < 1)
            {
                name = toes[i].name;
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);

                description += "  " + toes[i].subName + " " + name + ": " + Mathf.Floor(toes[i].health) + " / " + toes[i].maxHealth + "\n";
            }
        }

        return description;
    }

    public Leg leg;
    public Foot foot;
    public List<Toe> toes = new();

    public Femur femur;
    public Tibia tibia;

    public float efficiency = 0;
}