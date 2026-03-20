namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class ReliefValve : HydraulicComponent
    {
        public int MaxPressure { get; set; }

        public bool IsOpen { get; set; }

        public override int Process(int incomingPressure)
        {
            if (incomingPressure > MaxPressure)
            {
                CurrentPressure = MaxPressure;
                IsOpen = true;
                return incomingPressure - MaxPressure;
            }
            else
            {
                CurrentPressure = incomingPressure;
                IsOpen = false;
                return 0;
            }

        }

        public override string GetName()
        {
            return "ReliefValve";
        }

        public override string GetValue()
        {
            return $"{MaxPressure} psi";
        }
    }
}