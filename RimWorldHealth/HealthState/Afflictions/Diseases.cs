namespace ShadowOfRimWorldHealth;

public class RWFlu : RWDisease
{
    public RWFlu(CreatureState state, RWBodyPart part) : base(state, part)
    {
        name = "Flu";

        lethal = true;

        severityGain = 0.2488f;
        severityLoss = 0.4947f;

        immunityGain = 0.2388f;

        treatment = 0.0773f;

        treatmentTimes = 0.5f;
    }
}

public class RWInfection : RWDisease
{
    public RWInfection(CreatureState state, RWBodyPart part) : base(state, part)
    {
        name = "Infection";

        lethal = true;

        severityGain = 0.84f;
        severityLoss = 0.7f;

        immunityGain = 0.6441f;

        treatment = 0.53f;

        treatmentTimes = 0.5f;
    }
}