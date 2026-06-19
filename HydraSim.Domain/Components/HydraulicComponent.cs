using Newtonsoft.Json;

namespace HydraSim.Domain.Components
{
    public abstract class HydraulicComponent
    {
        public int Id { get; set; }
        public int SimulationId { get; set; }

        public int CX { get; set; }
        public int CY { get; set; }
        public int ComponentId { get; set; }

        public int CurrentPressure { get; protected set; }

        [JsonIgnore]
        public List<HydraulicComponent> Outputs { get; } = new();

        protected HydraulicComponent(int cx, int cy)
        {
            CX = cx;
            CY = cy;
        }

        protected HydraulicComponent() { }

        public abstract int Process(int incomingPressure);
        public abstract string GetName();
        public abstract string GetValue();
    }
}
