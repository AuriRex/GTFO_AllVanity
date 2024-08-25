using DropServer.VanityItems;
using GameData;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace AllVanity.Patches
{
    internal class Managed
    {
        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch(typeof(PersistentInventoryManager), nameof(PersistentInventoryManager.CommitPendingTransactions))]
        internal static class PersistentInventoryManager_CommitPendingTransactions_Patch
        {
            public static bool Prefix()
            {
                if (PersistentInventoryManager.m_dirty)
                {
                    SetupVanityInventory();
                    PersistentInventoryManager.m_dirty = false;
                    return false;
                }
                return true;
            }
        }

        public static void SetupVanityInventory()
        {
            Plugin.L.LogWarning("Setting up Vanity Item Inventory!");
            PersistentInventoryManager.Current.m_vanityItemsInventory.UpdateItems(CreateVanityPlayerData());
        }

        internal static VanityItemPlayerData CreateVanityPlayerData()
        {
            var vanity = new VanityItemPlayerData(ClassInjector.DerivedConstructorPointer<VanityItemPlayerData>());

            var allBlocks = GameDataBlockBase<VanityItemsTemplateDataBlock>.GetAllBlocks();

            var vanityArray = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DropServer.VanityItems.VanityItem>(allBlocks.Count);

            var c = 0;
            foreach (VanityItemsTemplateDataBlock block in allBlocks)
            {
                if (!Unlock.IsAllowedToUnlock(block))
                    continue;

                DropServer.VanityItems.VanityItem item = new DropServer.VanityItems.VanityItem(ClassInjector.DerivedConstructorPointer<DropServer.VanityItems.VanityItem>())
                {
                    Flags = InventoryItemFlags.Touched | InventoryItemFlags.Acknowledged,
                    ItemId = block.persistentID,
                };

                vanityArray[c] = item;
                c++;
            }

            vanity.Items = vanityArray;

            return vanity;
        }

    }
}
