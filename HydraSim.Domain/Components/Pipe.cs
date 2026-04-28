namespace HydraSim.Domain.Components
{
    public class Pipe : HydraulicComponent
    {
        public bool isCorner { get; set; }
        public int Rotation { get; set; } = 0;

        public override int Process(int incomingPressure)
        {
            return incomingPressure;
        }

        public override string GetName()
        {
            if (!isCorner)
                return "Pipe";
            else
                return "PipeCorner";
        }

        public override string GetValue() => null;

        public string GetPipeColor(int maxPressure)
        {
            int p = CurrentPressure;

            if (p < 0) return "#ff00ff";
            if (p == 0) return "#0000ff";
            if (p == 1) return "#ffff00";

            double ratio = (double)p / maxPressure;
            ratio = Math.Max(0, Math.Min(1, ratio));

            int red = 255;
            int green = (int)(255 * (1 - ratio));
            int blue = 0;

            return $"rgb({red},{green},{blue})";
        }
    }
}
