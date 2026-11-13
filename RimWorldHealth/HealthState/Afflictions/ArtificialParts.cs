namespace ShadowOfRimWorldHealth;

public class RWPegLeg : RWArtificialPartBase
{
    public RWPegLeg(CreatureState state, RWBodyPart part) : base(state, part)
    {
        name = "Peg leg";
        efficiency = 0.6f;
        descrition = "An installed peg leg. Allows the user to\n" +
            "walk again, albeit not very well\n\n" +
            "  - Part efficiency: 60%";
    }
}

public class RWWoodenHand : RWArtificialPartBase
{
    public RWWoodenHand(CreatureState state, RWBodyPart part) : base(state, part)
    {
        name = "Wooden hand";
        efficiency = 0.6f;
        descrition = "An installed wooden hand. Better than a\n" +
            "stump, but not by much.\n\n" +
            "  - Part efficiency: 60%";
    }
}

public class RWWoodenFoot : RWArtificialPartBase
{
    public RWWoodenFoot(CreatureState state, RWBodyPart part, string nameOverride = "foot") : base(state, part)
    {
        name = "Wooden " + nameOverride;
        efficiency = 0.8f;
        descrition = "An installed wooden " + nameOverride + ". Restores\n" +
            "some stability to the user, but without\n" +
            "finer motor skills.\n\n" +
            "  - Part efficiency: 80%";
    }
}

public class RWDenture : RWArtificialPartBase
{
    public RWDenture(CreatureState state, RWBodyPart part) : base(state, part)
    {
        name = "Denture";
        efficiency = 0.8f;
        descrition = "An installed denture. Allows for some\n" +
            "basic functionality like eating and talking.\n\n" +
            "  - Part efficiency: 80%";
    }
}