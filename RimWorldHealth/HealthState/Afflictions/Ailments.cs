namespace ShadowOfRimWorldHealth;

internal class RimAirInLungs : RimAilment
{
    public RimAirInLungs(CreatureState state, RimBodyPart part, float airInLungs) : base(state, part)
    {
        amount = airInLungs; //Used for determining how much oxygen is in the lungs, 1 being full and 0 being empty

        labelColour = labelColour = new(0.7f, 0.7f, 1f);
    }
}
internal class RimHypothermia : RimAilment
{
    public RimHypothermia(CreatureState state, RimBodyPart part, float hypothermia) : base(state, part)
    {
        amount = hypothermia; //Used for determining how far hypothermia has progressed
        lastAmount = hypothermia;

        labelColour = labelColour = new(0.8f, 0.8f, 1f);
    }
}
internal class RimToxicBuildup : RimAilment
{
    public RimToxicBuildup(CreatureState state, RimBodyPart part, float poison) : base(state, part)
    {
        amount = poison; //Used for determining how much Toxic Buildup there is
        lastAmount = poison;

        labelColour = labelColour = new(0.7f, 1f, 0.7f);
    }
}

internal class RimAnesthetic : RimAilment
{
    public RimAnesthetic(CreatureState state, RimBodyPart part, float ticks) : base(state, part)
    {
        amount = ticks; //Unlike other Ailments this is in ticks
        lastAmount = amount; //Used as a "max" for calculations

        labelColour = labelColour = new(0.9f, 1f, 0.35f);
    }
}