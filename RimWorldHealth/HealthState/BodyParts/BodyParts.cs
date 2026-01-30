using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RWBodyPart
{
    public RWBodyPart(RWPlayerHealthState state)
    {
        this.state = state;
    }

    public RWPlayerHealthState state;

    public string name = "Generic Body Part";

    public float health = 10f;

    public int quantity = 1;

    public float coverage = 100f;

    public List<int> connectedBodyChunks = new();
    public List<int> connectedAppendage = new();

    public string subPartOf = null;

    public bool isInternal = false;

    public List<string> group = new();

    public List<string> capacity = new();

    public string deathEffect = "Destroy";
}

internal class UpperTorso : RWBodyPart
{
    public UpperTorso(RWPlayerHealthState state) : base(state)
    {
        name = "Upper Torso";

        health = 20;

        connectedBodyChunks.Add(1);

        group.Add("upperTorso");

        deathEffect = "Death";
    }
}
internal class LowerTorso : RWBodyPart
{
    public LowerTorso(RWPlayerHealthState state) : base(state)
    {
        name = "Lower Torso";

        health = 20;

        connectedBodyChunks.Add(2);

        group.Add("lowerTorso");

        deathEffect = "CutInHalf";
    }
}
internal class Neck : RWBodyPart
{
    public Neck(RWPlayerHealthState state) : base(state)
    {
        name = "Neck";

        health = 25;

        coverage = 7.5f;

        connectedBodyChunks.Add(0);

        subPartOf = "Upper Torso";

        group.Add("Neck");

        capacity.Add("Eating");
        capacity.Add("Talking");
        capacity.Add("Breathing");

        deathEffect = "Decapitation";
    }
}
internal class Head : RWBodyPart
{
    public Head(RWPlayerHealthState state) : base(state)
    {
        name = "Head";

        health = 25;

        coverage = 80f;

        connectedBodyChunks.Add(0);

        subPartOf = "Neck";

        group.Add("UpperHead");
        group.Add("FullHead");

        deathEffect = "Decapitation";
    }
}
internal class Skull : RWBodyPart
{
    public Skull(RWPlayerHealthState state) : base(state)
    {
        name = "Brain";

        health = 25;

        subPartOf = "Head";

        isInternal = true;

        group.Add("UpperHead");
        group.Add("Eyes");
        group.Add("FullHead");

        deathEffect = "";
    }
}
internal class Brain : RWBodyPart
{
    public Brain(RWPlayerHealthState state) : base(state)
    {
        name = "Brain";

        health = 10;

        subPartOf = "Skull";

        isInternal = true;

        group.Add("UpperHead");
        group.Add("Eyes");
        group.Add("FullHead");

        capacity.Add("Consciousness");

        deathEffect = "Death";
    }
}
internal class Eye : RWBodyPart
{
    public Eye(RWPlayerHealthState state) : base(state)
    {
        name = "Brain";

        health = 10;

        quantity = 2;

        subPartOf = "Head";

        group.Add("Eyes");
        group.Add("FullHead");
    }
}