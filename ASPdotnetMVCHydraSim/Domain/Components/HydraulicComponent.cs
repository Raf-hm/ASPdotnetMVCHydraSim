namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public abstract class HydraulicComponent
    {
        public int CurrentPressure { get; set; }
        public abstract int Process(int incomingPressure);
        public abstract string GetName();
        public abstract string GetValue();
    }
}
