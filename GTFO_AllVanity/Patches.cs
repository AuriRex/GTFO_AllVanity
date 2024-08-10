using HarmonyLib;

namespace AllVanity
{
    internal class Patches
    {
        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch(typeof(PersistentInventoryManager), nameof(PersistentInventoryManager.CommitPendingTransactions))]
        internal static class PersistentInventoryManager_CommitPendingTransactions_Patch
        {
            private static bool _dirtyFlag = false;
            public static bool Prefix(PersistentInventoryManager __instance)
            {
                if (EntryPoint.noboostersLoaded)
                {
                    if (PersistentInventoryManager.m_dirty)
                    {
                        Unlock.SetupVanityInventory();
                        PersistentInventoryManager.m_dirty = false;
                        return false;
                    }
                    return true;
                }
                
                if (PersistentInventoryManager.m_dirty || __instance.m_vanityItemPendingTransactions.IsDirty())
                {
                    _dirtyFlag = true;
                    return true;
                }

                if (_dirtyFlag)
                {
                    BepInEx.Unity.IL2CPP.Utils.MonoBehaviourExtensions.StartCoroutine(__instance, Unlock.DelayedSetupVanity());
                    
                    _dirtyFlag = false;
                }

                return true;
            }
        }

    }
}
