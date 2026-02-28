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

            // Vacuum
            if (p < 0)
                return "#ff00ff";

            // Geen druk
            if (p == 0)
                return "#0000ff";

            // 1 druk = geel
            if (p == 1)
                return "#ffff00";

            // Boven 1 → vloeiende overgang geel → rood
            double ratio = (double)p / maxPressure;

            // Clamp tussen 0 en 1
            ratio = Math.Max(0, Math.Min(1, ratio));

            int red = 255;
            int green = (int)(255 * (1 - ratio));
            int blue = 0;

            return $"rgb({red},{green},{blue})";
        }
    }
}

