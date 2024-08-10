using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Linq;
using System.Reflection;

[assembly: AssemblyVersion(AllVanity.EntryPoint.VERSION)]
[assembly: AssemblyFileVersion(AllVanity.EntryPoint.VERSION)]
[assembly: AssemblyInformationalVersion(AllVanity.EntryPoint.VERSION)]

namespace AllVanity
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(NOBOOSTERS_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInIncompatibility(DEVIOUSLICK_GUID)]
    public class EntryPoint : BasePlugin
    {
        public const string GUID = "dev.aurirex.gtfo.allvanity";
        public const string NAME = "All Vanity";
        public const string VERSION = "1.0.0";

        public const string DEVIOUSLICK_GUID = "com.mccad00.AmongDrip";
        public const string NOBOOSTERS_GUID = "dev.aurirex.gtfo.noboosters";

        private Harmony _harmonyInstance;

        internal static ManualLogSource L;

        internal static bool noboostersLoaded = false;

        public override void Load()
        {
            L = Log;
            
            noboostersLoaded = IL2CPPChainloader.Instance.Plugins.Any(kvp => kvp.Key == NOBOOSTERS_GUID);

            Log.LogInfo("Applying Patches ...");

            _harmonyInstance = new Harmony(GUID);

            _harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}