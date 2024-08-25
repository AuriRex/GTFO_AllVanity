using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Linq;
using System.Reflection;

[assembly: AssemblyVersion(AllVanity.Plugin.VERSION)]
[assembly: AssemblyFileVersion(AllVanity.Plugin.VERSION)]
[assembly: AssemblyInformationalVersion(AllVanity.Plugin.VERSION)]

namespace AllVanity
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(NOBOOSTERS_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInIncompatibility(DEVIOUSLICK_GUID)]
    public class Plugin : BasePlugin
    {
        public const string GUID = "dev.aurirex.gtfo.allvanity";
        public const string NAME = "All Vanity";
        public const string VERSION = "1.2.0";

        public const string DEVIOUSLICK_GUID = "com.mccad00.AmongDrip";
        public const string NOBOOSTERS_GUID = "dev.aurirex.gtfo.noboosters";

        private Harmony _harmonyInstance;

        internal static ManualLogSource L;

        internal static bool noboostersLoaded = false;

        internal static string hexColorUnlocked = "faa";

        public override void Load()
        {
            L = Log;
            
            noboostersLoaded = IL2CPPChainloader.Instance.Plugins.Any(kvp => kvp.Key == NOBOOSTERS_GUID);

            _harmonyInstance = new Harmony(GUID);

            if (noboostersLoaded)
            {
                Log.LogInfo("NoBoosters is installed, harmony patching ...");
                _harmonyInstance.PatchAll(typeof(Patches.Managed.PersistentInventoryManager_CommitPendingTransactions_Patch));
            }
            else
            {
                Log.LogInfo("NoBoosters is NOT installed, native patching ...");
                Patches.Native.ApplyNative();
            }
        }
    }
}