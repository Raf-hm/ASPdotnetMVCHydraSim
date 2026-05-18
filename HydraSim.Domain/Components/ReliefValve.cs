using System.ComponentModel.DataAnnotations.Schema;

namespace HydraSim.Domain.Components
{
    public class ReliefValve : HydraulicComponent
    {
        public int MaxPressure { get; set; }

        [NotMapped]
        public bool IsOpen { get; set; }

        public ReliefValve(int cx, int cy, int maxPressure) : base(cx, cy)
        {
            MaxPressure = maxPressure;
        }

        public ReliefValve() { }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            return IsOpen ? incomingPressure : 0;
        }

        public override string GetName() => "ReliefValve";
        public override string GetValue() => $"{MaxPressure} psi max";
    }
}
