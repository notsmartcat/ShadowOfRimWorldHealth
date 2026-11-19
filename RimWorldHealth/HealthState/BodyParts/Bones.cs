namespace ShadowOfRimWorldHealth;

public class RimBone : RimBodyPart
{
    public RimBone(CreatureState state) : base(state)
    {
        isInternal = true;
        isSolid = true;
    }
}

public class Skull : RimBone
{
    public Skull(CreatureState state) : base(state)
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

public class Spine : RimBone
{
    public Spine(CreatureState state) : base(state)
    {
        name = "Spine";

        maxHealth = 25;
        health = 25;

        coverage = 2.5f;

        subPartOf = "Upper Torso";

        group.Add("upperTorso");

        capacity.Add("Moving");
        capacityImportance.Add(1);
    }
}
public class Ribcage : RimBone
{
    public Ribcage(CreatureState state) : base(state)
    {
        name = "Ribcage";

        maxHealth = 30;
        health = 30;

        coverage = 3.6f;

        subPartOf = "Upper Torso";

        group.Add("upperTorso");

        capacity.Add("Breathing");
        capacityImportance.Add(0.5f);

        deathEffect = "";
    }
}
public class Sternum : RimBone
{
    public Sternum(CreatureState state) : base(state)
    {
        name = "Sternum";

        maxHealth = 20;
        health = 20;

        coverage = 1.5f;

        subPartOf = "Upper Torso";

        group.Add("upperTorso");

        capacity.Add("Breathing");
        capacityImportance.Add(0.5f);

        deathEffect = "";
    }
}

public class Clavicle : RimBone
{
    public Clavicle(CreatureState state) : base(state)
    {
        name = "Clavicle";

        parentSubName = true;

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
public class Humerus : RimBone
{
    public Humerus(CreatureState state) : base(state)
    {
        name = "Humerus";

        parentSubName = true;

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Arm";

        group.Add("Arms");

        capacity.Add("Manipulation");
    }
}
public class Radius : RimBone
{
    public Radius(CreatureState state) : base(state)
    {
        name = "Radius";

        parentSubName = true;

        maxHealth = 20;
        health = 20;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Arm";

        group.Add("Arms");

        capacity.Add("Manipulation");
    }
}

public class Pelvis : RimBone
{
    public Pelvis(CreatureState state) : base(state)
    {
        name = "Pelvis";

        maxHealth = 25;
        health = 25;

        coverage = 2.5f;

        subPartOf = "Lower Torso";

        group.Add("lowerTorso");

        capacity.Add("Moving");
        capacityImportance.Add(1);

        deathEffect = "";
    }
}

public class Femur : RimBone
{
    public Femur(CreatureState state) : base(state)
    {
        name = "Femur";

        parentSubName = true;

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Leg";

        group.Add("Legs");

        capacity.Add("Moving");
    }
}
public class Tibia : RimBone
{
    public Tibia(CreatureState state) : base(state)
    {
        name = "Tibia";

        parentSubName = true;

        maxHealth = 25;
        health = 25;

        quantity = 2;

        coverage = 10f;

        subPartOf = "Leg";

        group.Add("Legs");

        capacity.Add("Moving");
    }
}