namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class pressuregauge : HydraulicComponent
    {
        private int _incomingPressure;
        public override int Process(int incomingPressure)
        {
            _incomingPressure = incomingPressure;
            return incomingPressure;
        }
        public override string GetInfo()
        {
            return $"PressureGauge {_incomingPressure}";
        }
    }
}
