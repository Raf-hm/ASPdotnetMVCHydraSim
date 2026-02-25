namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public abstract class HydraulicComponent
    {
        public abstract int Process(int incomingPressure);
        public abstract string GetInfo();
    }
}
