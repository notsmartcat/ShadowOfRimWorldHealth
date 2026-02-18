namespace ShadowOfRimWorldHealth;

internal class RWBone : RWBodyPart
{
    public RWBone(RWPlayerHealthState state) : base(state)
    {
        isInternal = true;
        isSolid = true;
    }
}

internal class Skull : RWBone
{
    public Skull(RWPlayerHealthState state) : base(state)
    {
        name = "Skull";

        maxHealth = 25;
        health = 25;

        coverage = 18f;

        subPartOf = "Head";

        group.Add("UpperHead");
        group.Add("Eyes");
        group.Add("FullHead");

        deathEffect = "";
    }
}

internal class Spine : RWBone
{
    public Spine(RWPlayerHealthState state) : base(state)
    {
        name = "Spine";

        maxHealth = 25;
        health = 25;

        coverage = 2.5f;

        subPartOf = "Upper Torso";

        group.Add("upperTorso");

        capacity.Add("Moving");
    }
}
internal class Ribcage : RWBone
{
    public Ribcage(RWPlayerHealthState state) : base(state)
    {
        name = "Ribcage";

        maxHealth = 30;
        health = 30;

        coverage = 3.6f;

        subPartOf = "Upper Torso";

        group.Add("upperTorso");

        capacity.Add("Breathing");

        deathEffect = "";
    }
}
internal class Sternum : RWBone
{
    public Sternum(RWPlayerHealthState state) : base(state)
    {
        name = "Sternum";

        maxHealth = 20;
        health = 20;

        coverage = 1.5f;

        subPartOf = "Upper Torso";

        group.Add("upperTorso");

        capacity.Add("Breathing");

        deathEffect = "";
    }
}

internal class Clavicle : RWBone
{
    public Clavicle(RWPlayerHealthState state) : base(state)
    {
        name = "Clavicle";

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 9f;

        subPartOf = "Shoulder";

        group.Add("upperTorso");

        capacity.Add("Manipulation");

        deathEffect = "";
    }
}
internal class Humerus : RWBone
{
    public Humerus(RWPlayerHealthState state) : base(state)
    {
        name = "Humerus";

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Arm";

        group.Add("Arms");

        capacity.Add("Manipulation");
    }
}
internal class Radius : RWBone
{
    public Radius(RWPlayerHealthState state) : base(state)
    {
        name = "Radius";

        maxHealth = 20;
        health = 20;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Arm";

        group.Add("Arms");

        capacity.Add("Manipulation");
    }
}

internal class Pelvis : RWBone
{
    public Pelvis(RWPlayerHealthState state) : base(state)
    {
        name = "Pelvis";

        maxHealth = 25;
        health = 25;

        coverage = 2.5f;

        subPartOf = "Lower Torso";

        group.Add("lowerTotso");

        capacity.Add("Moving");

        deathEffect = "";
    }
}

internal class Femur : RWBone
{
    public Femur(RWPlayerHealthState state) : base(state)
    {
        name = "Femur";

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Leg";

        group.Add("Legs");

        capacity.Add("Moving");
    }
}
internal class Tibia : RWBone
{
    public Tibia(RWPlayerHealthState state) : base(state)
    {
        name = "Tibia";

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Leg";

        group.Add("Legs");

        capacity.Add("Moving");
    }
}