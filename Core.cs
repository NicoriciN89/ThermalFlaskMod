using System.Collections.Generic;
using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Gear;

[assembly: MelonInfo(typeof(ThermalFlaskMod.Core), "ThermalFlaskMod", "1.1.0", "NicoriciN89")]
[assembly: MelonGame("Hinterland", "TheLongDark")]
[assembly: MelonPriority(200)]

namespace ThermalFlaskMod
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Settings.Initialize();
            LoggerInstance.Msg("ThermalFlaskMod loaded.");
        }
    }

    // The game exposes two separate vanilla fields for the flask's heat-loss
    // speed: m_PercentHeatLossPerMinuteIndoors / ...Outdoors. CalculateHeatLoss
    // is a native IL2CPP method that reads these fields directly rather than
    // through their C# get_ accessors, so patching the getters has no effect
    // on actual gameplay. Instead we patch CalculateHeatLoss itself: right
    // before it runs, we overwrite the fields (via their setters, which do
    // reach the native field) with the vanilla value scaled by the user's
    // slider (0.1x-5x), then restore the true vanilla value afterwards so the
    // cached "vanilla" we track next call is never itself a scaled value.
    [HarmonyPatch(typeof(InsulatedFlask), "CalculateHeatLoss")]
    internal static class Patch_Flask_CalculateHeatLoss
    {
        private static readonly Dictionary<int, float> VanillaIndoors = new Dictionary<int, float>();
        private static readonly Dictionary<int, float> VanillaOutdoors = new Dictionary<int, float>();

        private static void Prefix(InsulatedFlask __instance)
        {
            int id = __instance.GetInstanceID();

            if (!VanillaIndoors.TryGetValue(id, out float vanillaIndoors))
            {
                vanillaIndoors = __instance.m_PercentHeatLossPerMinuteIndoors;
                VanillaIndoors[id] = vanillaIndoors;
            }

            if (!VanillaOutdoors.TryGetValue(id, out float vanillaOutdoors))
            {
                vanillaOutdoors = __instance.m_PercentHeatLossPerMinuteOutdoors;
                VanillaOutdoors[id] = vanillaOutdoors;
            }

            __instance.m_PercentHeatLossPerMinuteIndoors = vanillaIndoors * Settings.Instance.coolingSpeedIndoors;
            __instance.m_PercentHeatLossPerMinuteOutdoors = vanillaOutdoors * Settings.Instance.coolingSpeedOutdoors;
        }

        private static void Postfix(InsulatedFlask __instance)
        {
            int id = __instance.GetInstanceID();

            // Restore true vanilla values so next Prefix's cache read isn't already scaled.
            if (VanillaIndoors.TryGetValue(id, out float vanillaIndoors))
                __instance.m_PercentHeatLossPerMinuteIndoors = vanillaIndoors;

            if (VanillaOutdoors.TryGetValue(id, out float vanillaOutdoors))
                __instance.m_PercentHeatLossPerMinuteOutdoors = vanillaOutdoors;
        }
    }
}
