namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Pump : HydraulicComponent
    {
        public int PressureOutput { get; set; }

        public override int Process(int incomingPressure)
        {
            return PressureOutput;
        }
        public override string GetInfo()
        {
            return $"Pump";
        }
    }
}
