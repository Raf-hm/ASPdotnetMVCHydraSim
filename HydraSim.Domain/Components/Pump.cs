namespace HydraSim.Domain.Components
{
    public class Pump : HydraulicComponent
    {
        private int _pressureOutput;

        public Pump(int cx, int cy, int pressureOutput) : base(cx, cy)
        {
            _pressureOutput = pressureOutput;
        }

        public Pump() { }

        public int PressureOutput
        {
            get => _pressureOutput;
            set => _pressureOutput = value;
        }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = PressureOutput;
            return PressureOutput;
        }

        public override string GetName() => "Pump";
        public override string GetValue() => $"{PressureOutput} psi";
    }
}
