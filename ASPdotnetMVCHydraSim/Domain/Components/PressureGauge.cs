namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class PressureGauge : HydraulicComponent
    {
        private int _incomingPressure;
        public override int Process(int incomingPressure)
        {
            _incomingPressure = incomingPressure;
            return incomingPressure;
        }
        public override string GetName()
        {
            return $"PressureGauge {_incomingPressure}";
        }
        public override string GetValue()
        {
            return $"{_incomingPressure}";
        }
    }
}
