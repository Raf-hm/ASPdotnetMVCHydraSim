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
    }
}

