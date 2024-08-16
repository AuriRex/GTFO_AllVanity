using HarmonyLib;

namespace AllVanity
{
    internal class Patches
    {
        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch(typeof(PersistentInventoryManager), nameof(PersistentInventoryManager.CommitPendingTransactions))]
        internal static class PersistentInventoryManager_CommitPendingTransactions_Patch
        {
            public static bool Prefix()
            {
                if (PersistentInventoryManager.m_dirty)
                {
                    Unlock.SetupVanityInventory();
                    PersistentInventoryManager.m_dirty = false;
                    return false;
                }
                return true;
            }
        }

    }
}
