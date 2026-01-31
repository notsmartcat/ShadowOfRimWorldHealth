using System.Collections.Generic;

namespace ShadowOfRimWorldHealth;

public class RWBodyPart
{
    public RWBodyPart(RWPlayerHealthState state)
    {
        this.state = state;
    }

    public RWPlayerHealthState state;

    //name is used for showing up in the Health Tab and for localization
    public string name = "Generic Body Part";

    //how much damage the bodyPart can take before being destroyed
    public float health = 10f;

    //if there is more then 1 quantity a second version of this is automatically created. The first bodypart will be given a "Right " to it's name and the second one will be given a "Left " 
    public int quantity = 1;

    //Coverage determines the chance to hit this body part. It refers to the percentage of the super-part that this part covers, before its own sub-parts claim their own percentage.
    //For example, if the base coverage of the super-part is 100%, and the coverage of the part is 20%, 20% of hits would hit the part, and 80% the super-part.
    //If the part had its own sub-part with 50% coverage, the chances would be 10% sub-part, 10% part, 80% super part.
    public float coverage = 100f;

    //connected boduchunks/appendages are reserver for the first bodypart to be hit whenever a bodyvhunk/appendage is hit
    public List<int> connectedBodyChunks = new();
    public List<int> connectedAppendage = new();

    //determines what bodypart this bodypart is connected to
    public string subPartOf = null;

    //if the bodypart is internal the Subpart will also be hit whenever this bodypart is hit
    public bool isInternal = false;

    //What armour group covers this body part.
    public List<string> group = new();

    //what stats this bodypart affects whenever damaged or destroyed
    public List<string> capacity = new();

    //if a bodypart has a special function whenever destroyed, such as cutting in half or decapitation
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

        group.Add("UpperHead");
        group.Add("FullHead");

        deathEffect = "Decapitation";
    }
}
internal class Skull : RWBodyPart
{
    public Skull(RWPlayerHealthState state) : base(state)
    {
        name = "Skull";

        health = 25;

        coverage = 18f;

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

        coverage = 80f;

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
        name = "Eye";

        health = 10;

        quantity = 2;

        coverage = 7f;

        subPartOf = "Head";

        group.Add("Eyes");
        group.Add("FullHead");
    }
}