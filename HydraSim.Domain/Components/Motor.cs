namespace HydraSim.Domain.Components
{
    public class Motor : HydraulicComponent
    {
        private int _requiredPressure;

        public Motor(int cx, int cy, int requiredPressure) : base(cx, cy)
        {
            _requiredPressure = requiredPressure;
        }

        public Motor() { }

        public int RequiredPressure
        {
            get => _requiredPressure;
            set => _requiredPressure = value;
        }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return incomingPressure <= 0 ? 0 : incomingPressure - RequiredPressure;
        }

        public override string GetName() => "Motor";
        public override string GetValue() => $"{RequiredPressure} psi";
    }
}
