#nullable disable
using ModSettings;

namespace ThermalFlaskMod
{
    internal class FlaskSettings : JsonModSettings
    {
        [Section("TF.SEC_INDOOR", Localize = true)]

        [Name("TF.INDOOR_SPEED", Localize = true)]
        [Description("TF.DESC_INDOOR_SPEED", Localize = true)]
        [Slider(0.1f, 5f, 490, NumberFormat = "{0:0.00}x")]
        public float coolingSpeedIndoors = 1f;

        [Section("TF.SEC_OUTDOOR", Localize = true)]

        [Name("TF.OUTDOOR_SPEED", Localize = true)]
        [Description("TF.DESC_OUTDOOR_SPEED", Localize = true)]
        [Slider(0.1f, 5f, 490, NumberFormat = "{0:0.00}x")]
        public float coolingSpeedOutdoors = 1f;
    }

    internal static class Settings
    {
        internal static FlaskSettings Instance;

        internal static void Initialize()
        {
            Instance = new FlaskSettings();
            Instance.AddToModSettings("Thermal Flask");
        }
    }
}
