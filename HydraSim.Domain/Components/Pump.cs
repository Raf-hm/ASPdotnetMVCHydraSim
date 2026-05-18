using System.ComponentModel.DataAnnotations.Schema;

namespace HydraSim.Domain.Components
{
    public class Pump : HydraulicComponent
    {
        [NotMapped]
        public int PressureOutput { get; set; }

        public Pump(int cx, int cy, int pressureOutput) : base(cx, cy)
        {
            PressureOutput = pressureOutput;
        }

        public Pump() { }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = PressureOutput;
            return PressureOutput;
        }

        public override string GetName() => "Pump";
        public override string GetValue() => $"{PressureOutput} psi";
    }
}
