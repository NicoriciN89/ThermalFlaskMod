using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ThermalFlaskMod
{
    // Loads TF.* translations from localization.json shipped alongside the DLL.
    // User override: UserData/ThermalFlaskMod/localization.json takes priority.
    // Falls back to built-in English strings if the JSON is missing or corrupt.
    internal static class LocalizationManager
    {
        private static Dictionary<string, Dictionary<string, string>> _data;

        internal static Dictionary<string, Dictionary<string, string>> Data
        {
            get { return _data ?? (_data = Load()); }
        }

        internal static void Reload() => _data = null;

        private const string EmbeddedResourceName = "ThermalFlaskMod.localization.json";

        private static Dictionary<string, Dictionary<string, string>> Load()
        {
            string dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            string userPath = Path.Combine(
                Path.GetDirectoryName(dllDir) ?? dllDir,
                "UserData", "ThermalFlaskMod", "localization.json");

            if (File.Exists(userPath))
            {
                var fromFile = TryLoadJson(File.ReadAllText(userPath, Encoding.UTF8));
                if (fromFile != null)
                {
                    MelonLogger.Msg($"[ThermalFlaskMod] Localization override loaded from: {userPath}");
                    return fromFile;
                }
            }

            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream(EmbeddedResourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var fromEmbedded = TryLoadJson(reader.ReadToEnd());
                if (fromEmbedded != null)
                {
                    MelonLogger.Msg("[ThermalFlaskMod] Localization loaded from embedded resource.");
                    return fromEmbedded;
                }
            }
            else
            {
                MelonLogger.Warning($"[ThermalFlaskMod] Embedded resource '{EmbeddedResourceName}' not found — using built-in English strings.");
            }

            return FallbackEnglish;
        }

        private static Dictionary<string, Dictionary<string, string>> TryLoadJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[ThermalFlaskMod] Failed to parse localization JSON: {ex.Message}");
                return null;
            }
        }

        private static readonly Dictionary<string, Dictionary<string, string>> FallbackEnglish = new()
        {
            ["English"] = new()
            {
                ["TF.SEC_INDOOR"] = "Heat retention indoors",
                ["TF.SEC_OUTDOOR"] = "Heat retention outdoors",
                ["TF.INDOOR_SPEED"] = "Heat retention speed indoors",
                ["TF.OUTDOOR_SPEED"] = "Heat retention speed outdoors",
                ["TF.DESC_INDOOR_SPEED"] = "How fast the flask cools down while indoors, relative to vanilla. Lower = stays warm longer.",
                ["TF.DESC_OUTDOOR_SPEED"] = "How fast the flask cools down while outdoors, relative to vanilla. Lower = stays warm longer.",
            }
        };

        internal static string Get(string key)
        {
            string lang = Localization.Language ?? "English";
            var data = Data;
            if (data.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out string val))
                return val;
            if (data.TryGetValue("English", out var en) && en.TryGetValue(key, out string enVal))
                return enVal;
            return key;
        }
    }

    [HarmonyPatch(typeof(Localization), nameof(Localization.Get))]
    internal static class LocalizationPatch
    {
        static void Postfix(string __0, ref string __result)
        {
            if (__0 == null || !__0.StartsWith("TF."))
                return;
            __result = LocalizationManager.Get(__0);
        }
    }

    // Patches ModSettings.DescriptionHolder.get_Text — intercepts the description
    // read at display time, when the current language is already known.
    [HarmonyPatch]
    internal static class DescriptionTextTranslatePatch
    {
        static MethodBase TargetMethod() =>
            AccessTools.PropertyGetter(
                AccessTools.TypeByName("ModSettings.DescriptionHolder"), "Text");

        static void Postfix(ref string __result)
        {
            if (__result == null || !__result.StartsWith("TF."))
                return;
            __result = LocalizationManager.Get(__result);
        }
    }
}
