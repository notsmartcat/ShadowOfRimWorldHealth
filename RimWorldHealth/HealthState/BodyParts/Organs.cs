namespace ShadowOfRimWorldHealth;

internal class RWOrgan : RWBodyPart
{
    public RWOrgan(RWPlayerHealthState state) : base(state)
    {
        isInternal = true;
    }
}

internal class Brain : RWOrgan
{
    public Brain(RWPlayerHealthState state) : base(state)
    {
        name = "Brain";

        maxHealth = 10;
        health = 10;

        coverage = 80f;

        subPartOf = "Skull";

        group.Add("UpperHead");
        group.Add("Eyes");
        group.Add("FullHead");

        capacity.Add("Consciousness");

        deathEffect = "Death";
    }
}

internal class Stomach : RWOrgan
{
    public Stomach(RWPlayerHealthState state) : base(state)
    {
        name = "Stomach";

        maxHealth = 20;
        health = 20;

        coverage = 2.5f;

        subPartOf = "upperTorso";

        group.Add("upperTorso");

        capacity.Add("Digestion");
    }
}
internal class Heart : RWOrgan
{
    public Heart(RWPlayerHealthState state) : base(state)
    {
        name = "Heart";

        maxHealth = 15;
        health = 15;

        coverage = 2f;

        subPartOf = "upperTorso"; //Might make this Ribcage

        group.Add("upperTorso");

        capacity.Add("Blood Pumping");

        deathEffect = "Death";
    }
}
internal class Lung : RWOrgan
{
    public Lung(RWPlayerHealthState state) : base(state)
    {
        name = "Lung";

        maxHealth = 15;
        health = 15;

        quantity = 2;

        coverage = 2.5f;

        subPartOf = "upperTorso";

        group.Add("upperTorso");

        capacity.Add("Breathing");
    }
}
internal class Kidney : RWOrgan
{
    public Kidney(RWPlayerHealthState state) : base(state)
    {
        name = "Kidney";

        maxHealth = 15;
        health = 15;

        quantity = 2;

        coverage = 1.7f;

        subPartOf = "upperTorso";

        group.Add("upperTorso");

        capacity.Add("Blood Filtration");
    }
}
internal class Liver : RWOrgan
{
    public Liver(RWPlayerHealthState state) : base(state)
    {
        name = "Liver";

        maxHealth = 20;
        health = 20;

        coverage = 2.5f;

        subPartOf = "upperTorso";

        group.Add("upperTorso");

        capacity.Add("Digestion");
        capacity.Add("Blood Filtration");

        deathEffect = "Death"; //might make this one not cause death and slowly regenerate
    }
}