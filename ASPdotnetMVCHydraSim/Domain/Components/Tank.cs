namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Tank : HydraulicComponent
    {
        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return -1;
        }

        public override string GetName() => "Tank";
        public override string GetValue() => null;
    }
}