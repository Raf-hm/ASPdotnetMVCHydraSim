namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Motor : HydraulicComponent
    {
        public int RequiredPressure { get; set; } 

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;

            return incomingPressure - RequiredPressure;
        }

        public override string GetName()
        {
            return "Motor";
        }

        public override string GetValue()
        {
            return $"{RequiredPressure} psi";
        }
    }
}