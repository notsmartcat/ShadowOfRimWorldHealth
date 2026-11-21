using System;
using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RimOperation(string name, string subName, RimBodyPart part, int[] medicalRequirments, Type requiredObject)
{
    public string name = name;
    public string subName = subName;

    public RimBodyPart part = part;

    public bool isSuspended = false;

    public int medicalRequired = medicalRequirments[0]; //The Medical level required to be able to operate.
    public int medicineAmount = medicalRequirments[1]; //Amount of medicine that is required to operate.
    public int medicineQuality = medicalRequirments[2]; //The minimum quality of medicine required to operate.

    public Type requiredObject = requiredObject; //The object required to operate, the object can either be on the floor near where the operation happens or can be held by the surgeon.
}

public class RimLimbReplacement(string name, string subName, RimBodyPart part, int[] medicalRequirments, Type requiredObject, string limbName, string limbSubName, Func<RimArtificialPartBase> replacement) : RimOperation(name, subName, part, medicalRequirments, requiredObject)
{
    public string limbName = limbName;
    public string limbSubName = limbSubName;

    public Func<RimArtificialPartBase> replacement = replacement;
}