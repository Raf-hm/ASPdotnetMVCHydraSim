namespace HydraSim.Domain.Components
{
    public class Pipe : HydraulicComponent
    {
        private int _rotation;

        public Pipe(int cx, int cy, bool isCorner = false, int rotation = 0) : base(cx, cy)
        {
            IsCorner = isCorner;
            _rotation = rotation;
        }

        public Pipe() { }

        public bool IsCorner { get; set; }

        public int Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return incomingPressure;
        }

        public override string GetName() => IsCorner ? "PipeCorner" : "Pipe";
        public override string GetValue() => null;

        public string GetPipeColor(int maxPressure)
        {
            int p = CurrentPressure;
            if (p < 0)  return "#ff00ff";
            if (p == 0) return "#0000ff";
            if (p == 1) return "#ffff00";

            double ratio = Math.Max(0, Math.Min(1, (double)p / maxPressure));
            int green = (int)(255 * (1 - ratio));
            return $"rgb(255,{green},0)";
        }
    }
}
