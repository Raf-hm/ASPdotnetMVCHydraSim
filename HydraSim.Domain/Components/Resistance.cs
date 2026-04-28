namespace HydraSim.Domain.Components
{
    public class Resistance : HydraulicComponent
    {
        private int _pressureDrop;

        public Resistance(int cx, int cy, int pressureDrop) : base(cx, cy)
        {
            _pressureDrop = pressureDrop;
        }

        public Resistance() { }

        public int PressureDrop
        {
            get => _pressureDrop;
            set => _pressureDrop = value;
        }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return incomingPressure - PressureDrop;
        }

        public override string GetName() => "Resistance";
        public override string GetValue() => $"{PressureDrop} psi drop";
    }
}
