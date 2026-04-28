namespace HydraSim.Domain.Components
{
    public class ReliefValve : HydraulicComponent
    {
        private int _maxPressure;

        public ReliefValve(int cx, int cy, int maxPressure) : base(cx, cy)
        {
            _maxPressure = maxPressure;
        }

        public ReliefValve() { }

        public int MaxPressure
        {
            get => _maxPressure;
            set => _maxPressure = value;
        }

        public bool IsOpen { get; set; }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return IsOpen ? 0 : incomingPressure;
        }

        public override string GetName() => "ReliefValve";
        public override string GetValue() => $"{MaxPressure} psi max";
    }
}
