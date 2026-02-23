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

    //subName is used in case there are multiple of the same bodyPart to diffirintiate between them, the subName is always added on before the name, most common subNames are "Right" and "Left"
    public string subName = "";

    //the parentSubName bool determines if the bodypart's parent also uses the same subName. for example a right hand's parent (right arm) will use a sub name while a right shoulder's parent (Upper Torso) will not
    public bool parentSubName = false;

    //Max Health, 
    public float maxHealth = 10f;

    //how much damage the bodyPart can take before being destroyed, actual health is calculated in the HealthState
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

    public bool isSolid = false;

    //delicate bodyparts (by dafault only eyes) will have a way higher chance to gain scars
    public bool isDelicate = false;

    //What armour group covers this body part.
    public List<string> group = new();

    //what stats this bodypart affects whenever damaged or destroyed
    public List<string> capacity = new();

    //if a bodypart has a special function whenever destroyed, such as cutting in half or decapitation
    public string deathEffect = "Destroy";

    public float efficiency = 1;

    public List<RWAffliction> afflictions = new();
}

internal class UpperTorso : RWBodyPart
{
    public UpperTorso(RWPlayerHealthState state) : base(state)
    {
        name = "Upper Torso";

        maxHealth = 40;
        health = 40;

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

        maxHealth = 40;
        health = 40;

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

        maxHealth = 25;
        health = 25;

        coverage = 7.5f;

        connectedBodyChunks.Add(0);
        connectedBodyChunks.Add(1);

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

        maxHealth = 25;
        health = 25;

        coverage = 80f;

        subPartOf = "Neck";

        connectedBodyChunks.Add(0);

        group.Add("UpperHead");
        group.Add("FullHead");

        deathEffect = "Death";
    }
}
internal class Eye : RWBodyPart
{
    public Eye(RWPlayerHealthState state) : base(state)
    {
        name = "Eye";

        maxHealth = 10;
        health = 10;

        quantity = 2;

        coverage = 7f;

        subPartOf = "Head";

        isDelicate = true;

        group.Add("FullHead");
        group.Add("Eyes");

        capacity.Add("Sight");
    }
}
internal class Ear : RWBodyPart
{
    public Ear(RWPlayerHealthState state) : base(state)
    {
        name = "Ear";

        maxHealth = 12;
        health = 12;

        quantity = 2;

        coverage = 7f;

        subPartOf = "Head";

        group.Add("UpperHead");
        group.Add("FullHead");

        capacity.Add("Hearing");
    }
}
internal class Nose : RWBodyPart
{
    public Nose(RWPlayerHealthState state) : base(state)
    {
        name = "Nose";

        maxHealth = 10;
        health = 10;

        coverage = 10f;

        subPartOf = "Head";

        group.Add("FullHead");
    }
}
internal class Jaw : RWBodyPart
{
    public Jaw(RWPlayerHealthState state) : base(state)
    {
        name = "Jaw";

        maxHealth = 20;
        health = 20;

        coverage = 15f;

        subPartOf = "Head";

        group.Add("FullHead");
        group.Add("Mouth");

        capacity.Add("Eating");
        capacity.Add("Talking");
    }
}
internal class Tongue : RWBodyPart
{
    public Tongue(RWPlayerHealthState state) : base(state)
    {
        name = "Jaw";

        coverage = 0.1f;

        subPartOf = "Jaw";

        isInternal = true;

        group.Add("FullHead");
        //group.Add("Mouth");

        capacity.Add("Talking");
    }
}

internal class Shoulder : RWBodyPart
{
    public Shoulder(RWPlayerHealthState state) : base(state)
    {
        name = "Shoulder";

        maxHealth = 30;
        health = 30;

        quantity = 2;

        coverage = 12f;

        subPartOf = "Upper Torso";

        group.Add("Shoulders");

        capacity.Add("Manipulation");
    }
}
internal class Arm : RWBodyPart
{
    public Arm(RWPlayerHealthState state) : base(state)
    {
        name = "Arm";

        parentSubName = true;

        maxHealth = 30;
        health = 30;

        quantity = 2;

        coverage = 77f;

        subPartOf = "Shoulder";

        group.Add("Arms");

        capacity.Add("Manipulation");
    }
}
internal class Hand : RWBodyPart
{
    public Hand(RWPlayerHealthState state) : base(state)
    {
        name = "Hand";

        parentSubName = true;

        maxHealth = 20;
        health = 20;

        quantity = 2;

        coverage = 14f;

        subPartOf = "Arm";

        group.Add("Hands");

        capacity.Add("Manipulation");
    }
}
internal class Finger : RWBodyPart
{
    public Finger(RWPlayerHealthState state) : base(state)
    {
        name = "Finger";

        parentSubName = true;

        maxHealth = 8;
        health = 8;

        quantity = 10;

        coverage = 8f;

        subPartOf = "Hand";

        group.Add("Hands");

        capacity.Add("Manipulation");
    }
}

internal class Leg : RWBodyPart
{
    public Leg(RWPlayerHealthState state) : base(state)
    {
        name = "Leg";

        maxHealth = 30;
        health = 30;

        quantity = 2;

        coverage = 14f;

        subPartOf = "Lower Torso";

        group.Add("Legs");

        capacity.Add("Moving");
    }
}
internal class Foot : RWBodyPart
{
    public Foot(RWPlayerHealthState state) : base(state)
    {
        name = "Foot";

        parentSubName = true;

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Leg";

        group.Add("Feet");

        capacity.Add("Moving");
    }
}
internal class Toe : RWBodyPart
{
    public Toe(RWPlayerHealthState state) : base(state)
    {
        name = "Toe";

        parentSubName = true;

        maxHealth = 8;
        health = 8;

        quantity = 10;

        coverage = 9f;

        subPartOf = "Foot";

        group.Add("Feet");

        capacity.Add("Moving");
    }
}

internal class Tail : RWBodyPart
{
    public Tail(RWPlayerHealthState state) : base(state)
    {
        name = "Tail";

        maxHealth = 20;
        health = 20;

        coverage = 10f;

        subPartOf = "Lower Torso";

        group.Add("lowerTorso");
    }
}