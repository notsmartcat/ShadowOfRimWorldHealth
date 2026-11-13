using System;

namespace ShadowOfRimWorldHealth;

public class RWOperation(string name, string subName, RWBodyPart part)
{
    public string name = name;
    public string subName = subName;

    public RWBodyPart part = part;

    public bool isSuspended = false;
}

public class RWLimbReplacement(string name, string subName, RWBodyPart part, string limbName, string limbSubName, Func<RWArtificialPartBase> replacement) : RWOperation(name, subName, part)
{
    public string limbName = limbName;
    public string limbSubName = limbSubName;

    public Func<RWArtificialPartBase> replacement = replacement;
}