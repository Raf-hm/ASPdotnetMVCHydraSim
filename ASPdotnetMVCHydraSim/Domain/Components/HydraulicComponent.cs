using Newtonsoft.Json;

namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public abstract class HydraulicComponent
    {
        [JsonIgnore]
        public List<HydraulicComponent> Outputs { get; set; } = new();
        public int CX { get; set; }
        public int CY { get; set; }
        public int CurrentPressure { get; set; }
        public int ComponentId { get; set; }
        public abstract int Process(int incomingPressure);
        public abstract string GetName();
        public abstract string GetValue();
    }
}