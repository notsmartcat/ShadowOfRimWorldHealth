namespace ShadowOfRimWorldHealth;

public class SlugcatFinger : Finger
{
    public SlugcatFinger(CreatureState state) : base(state)
    {
        quantity = 6;
    }
}

public class SlugcatPaw : Foot
{
    public SlugcatPaw(CreatureState state) : base(state)
    {
        name = "Paw";
    }
}
public class SlugcatToe : Toe
{
    public SlugcatToe(CreatureState state) : base(state)
    {
        name = "Toe";

        quantity = 4;

        subPartOf = "Paw";
    }
}