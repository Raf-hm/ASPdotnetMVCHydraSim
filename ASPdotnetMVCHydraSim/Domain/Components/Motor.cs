namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Motor : HydraulicComponent
    {
        public override int Process(int incomingPressure)
        {
            return incomingPressure;
        }

        public override string GetName()
        {
            return $"Motor";
        }
        public override string GetValue()
        {
            return null;
        }
    }
}
