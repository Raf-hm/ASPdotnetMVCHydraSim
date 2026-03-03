namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Resistance : HydraulicComponent
    {
        public int PressureDrop { get; set; }

        public override int Process(int incomingPressure)
        {
            return incomingPressure - PressureDrop;
        }

        public override string GetName()
        {
            return $"Resistance";
        }
        public override string GetValue()
        {
            return $"{PressureDrop}";
        }
    }
}
