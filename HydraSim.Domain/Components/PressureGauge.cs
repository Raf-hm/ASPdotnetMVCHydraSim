namespace HydraSim.Domain.Components
{
    public class PressureGauge : HydraulicComponent
    {
        public PressureGauge(int cx, int cy) : base(cx, cy) { }
        public PressureGauge() { }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return incomingPressure;
        }

        public override string GetName() => $"PressureGauge {CurrentPressure}";
        public override string GetValue() => $"{CurrentPressure} psi";
    }
}
