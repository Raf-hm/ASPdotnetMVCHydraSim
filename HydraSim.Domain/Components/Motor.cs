namespace HydraSim.Domain.Components
{
    public class Motor : HydraulicComponent
    {
        public int RequiredPressure { get; set; }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            int result = incomingPressure - RequiredPressure;
            return incomingPressure <= 0 ? 0 : result;
        }

        public override string GetName() => "Motor";
        public override string GetValue() => $"{RequiredPressure} psi";
    }
}
