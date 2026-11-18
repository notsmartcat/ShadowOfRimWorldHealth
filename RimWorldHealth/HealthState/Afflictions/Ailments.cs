namespace ShadowOfRimWorldHealth;

internal class RWAirInLungs : RWAilment
{
    public RWAirInLungs(CreatureState state, RWBodyPart part, float airInLungs) : base(state, part)
    {
        amount = airInLungs; //TendQuality is used for determining how much oxygen is in the lungs, 1 being full and 0 being empty
    }
}
internal class RWHypothermia : RWAilment
{
    public RWHypothermia(CreatureState state, RWBodyPart part, float hypothermia) : base(state, part)
    {
        amount = hypothermia; //TendQuality is used for determining how far hypothermia has progressed
        lastAmount = hypothermia;
    }
}
internal class RWToxicBuildup : RWAilment
{
    public RWToxicBuildup(CreatureState state, RWBodyPart part, float poison) : base(state, part)
    {
        amount = poison; //TendQuality is used for determining how much Toxic Buildup there is
        lastAmount = poison;
    }
}