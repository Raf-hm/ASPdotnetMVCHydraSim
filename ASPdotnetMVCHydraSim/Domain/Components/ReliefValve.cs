namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class ReliefValve : HydraulicComponent
    {
        public override int Process(int incomingPressure)
        {
            return incomingPressure;
        }

        public override string GetName()
        {
            return $"Pipe";
        }
        public override string GetValue()
        {
            return null;
        }
    }
}
