using Newtonsoft.Json;

namespace HydraSim.Domain.Components
{
    public abstract class HydraulicComponent
    {
        private int _currentPressure;
        private int _cx;
        private int _cy;
        private int _componentId;

        protected HydraulicComponent(int cx, int cy)
        {
            _cx = cx;
            _cy = cy;
        }

        protected HydraulicComponent() { }

        public int CX
        {
            get => _cx;
            set => _cx = value;
        }

        public int CY
        {
            get => _cy;
            set => _cy = value;
        }

        public int ComponentId
        {
            get => _componentId;
            set => _componentId = value;
        }

        public int CurrentPressure
        {
            get => _currentPressure;
            protected set => _currentPressure = value;
        }

        [JsonIgnore]
        public List<HydraulicComponent> Outputs { get; } = new();

        public abstract int Process(int incomingPressure);
        public abstract string GetName();
        public abstract string GetValue();
    }
}
