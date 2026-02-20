using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

internal class ArmSet
{
    public float Efficiency(float offsets = 0, float postFactors = 1)
    {
        float fingerEfficiency = 0;

        for (int i = 0; i < fingers.Count; i++)
        {
            fingerEfficiency += fingers[i].efficiency;
        }

        efficiency = (arm != null ? arm.efficiency : 0) * (shoulder != null ? shoulder.efficiency : 0) * (clavicle != null ? clavicle.efficiency : 0) * (humerus != null ? humerus.efficiency : 0) * (radius != null ? radius.efficiency : 0) * (hand != null ? hand.efficiency : 0) * ((fingerEfficiency * (0.8f / fingers.Count)) + 0.2f);

        efficiency = (efficiency + offsets) * postFactors;

        return efficiency;
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

internal class LegSet
{
    public float Efficiency(RWPlayerHealthState state = null, float offsets = 0, float postFactors = 1)
    {
        float toeEfficiency = 0;

        for (int i = 0; i < toes.Count; i++)
        {
            toeEfficiency += toes[i].efficiency;
        }

        efficiency = (leg != null ? leg.efficiency : 0) * (tibia != null ? tibia.efficiency : 0) * (femur != null ? femur.efficiency : 0) * (foot != null ? foot.efficiency : 0) * ((toeEfficiency * (0.4f / toes.Count)) + 0.6f);

        efficiency = (((state != null ? (1 + (state.bloodPumping - 1f) * 0.2f) * (1f + (state.breathing - 1) * 0.2f) : 0) * efficiency) + offsets) * postFactors;

        return efficiency;
    }

    public Leg leg;
    public Foot foot;
    public List<Toe> toes = new();

    public Femur femur;
    public Tibia tibia;

    public float efficiency = 0;
}