namespace HydraSim.Domain.Components
{
    public class Tank : HydraulicComponent
    {
        public Tank(int cx, int cy) : base(cx, cy) { }
        public Tank() { }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return -1;
        }

        public override string GetName() => "Tank";
        public override string GetValue() => null;
    }
}
