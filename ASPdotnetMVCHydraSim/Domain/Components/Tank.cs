namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Tank : HydraulicComponent
    {
        public override int Process(int incomingPressure)
        {
            return incomingPressure;
        }
        public override string GetName()
        {
            return $"Tank";
        }
        public override string GetValue()
        {
            return null;
        }
    }
}
