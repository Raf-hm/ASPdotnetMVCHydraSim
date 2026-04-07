namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class ReliefValve : HydraulicComponent
    {
        public int MaxPressure { get; set; }
        public bool IsOpen { get; set; }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return IsOpen ? 0 : incomingPressure;
        }

        public override string GetName() => "ReliefValve";
        public override string GetValue() => $"{MaxPressure} psi";
    }
}