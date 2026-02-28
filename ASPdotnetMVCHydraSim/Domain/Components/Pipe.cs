namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Pipe : HydraulicComponent
    {
        public override int Process(int incomingPressure)
        {
            return incomingPressure;
        }

        public override string GetInfo()
        {
            return $"Pipe";
        }

        public string GetPipeColor(int maxPressure)
        {
            int p = CurrentPressure;

            if (p < 0)
                return "#ff00ff";

            if (p == 0)
                return "#0000ff";

            if (p == 1)
                return "#ffff00";

            double ratio = (double)p / maxPressure;

            ratio = Math.Max(0, Math.Min(1, ratio));

            int red = 255;
            int green = (int)(255 * (1 - ratio));
            int blue = 0;

            return $"rgb({red},{green},{blue})";
        }
    }
}

