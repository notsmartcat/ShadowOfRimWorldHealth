namespace ShadowOfRimWorldHealth;

internal class RWAirInLungs : RWInformational
{
    public RWAirInLungs(CreatureState state, RWBodyPart part, float airInLungs) : base(state, part)
    {
        tendQuality = airInLungs; //TendQuality is used for determining how much oxygen is in the lungs, 1 being full and 0 being empty
    }
}

internal class RWHypothermia : RWInformational
{
    public RWHypothermia(CreatureState state, RWBodyPart part, float hypothermia) : base(state, part)
    {
        tendQuality = hypothermia; //TendQuality is used for determining how far hypothermia has progressed
        lastHypothermia = hypothermia;
    }

    public float lastHypothermia;
}