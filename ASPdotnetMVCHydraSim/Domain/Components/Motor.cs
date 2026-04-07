namespace ASPdotnetMVCHydraSim.Domain.Components
{
    public class Motor : HydraulicComponent
    {
        public int RequiredPressure { get; set; }

        public override int Process(int incomingPressure)
        {
            CurrentPressure = incomingPressure;
            // Negatieve druk alleen blokkeren als er geen druk op staat (RV open situatie)
            int result = incomingPressure - RequiredPressure;
            return incomingPressure <= 0 ? 0 : result;
        }

        public override string GetName() => "Motor";
        public override string GetValue() => $"{RequiredPressure} psi";
    }
}