namespace ShadowOfRimWorldHealth;

public class RimFlu : RimDisease
{
    public RimFlu(CreatureState state, RimBodyPart part) : base(state, part)
    {
        name = "Flu";

        lethal = true;

        severityGain = 0.2488f;
        severityLoss = 0.4947f;

        immunityGain = 0.2388f;

        treatment = 0.0773f;

        treatmentTimes = 0.5f;

        labelColour = new(0.8f, 0.8f, 0.35f);
    }
}

public class RimInfection : RimDisease
{
    public RimInfection(CreatureState state, RimBodyPart part) : base(state, part)
    {
        name = "Infection";

        lethal = true;

        severityGain = 0.84f;
        severityLoss = 0.7f;

        immunityGain = 0.6441f;

        treatment = 0.53f;

        treatmentTimes = 0.5f;

        labelColour = new(0.8f, 0.8f, 0.35f);
    }
}