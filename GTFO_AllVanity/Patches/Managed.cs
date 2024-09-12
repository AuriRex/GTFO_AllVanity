using AllVanity.Interop;
using DropServer.VanityItems;
using GameData;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Collections.Generic;

namespace AllVanity.Patches
{
    internal class Managed
    {
        [HarmonyPatch(typeof(PersistentInventoryManager), nameof(PersistentInventoryManager.CommitPendingTransactions))]
        internal static class PersistentInventoryManager_CommitPendingTransactions_Patch
        {
            [HarmonyPriority(Priority.HigherThanNormal)]
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

        [HarmonyPatch(typeof(PersistentInventoryManager), nameof(PersistentInventoryManager.TouchVanityItem))]
        internal static class PersistentInventoryManager_TouchVanityItem_Patch
        {
            public static bool Prefix(uint vanityItemId)
            {
                // We combine Ack and Touch here because there is no good way to patch into anything that acknowledges without breaking stuff :,)
                SimpleProgressionInterop.TouchAndAckIds(vanityItemId);
                PersistentInventoryManager.m_dirty = true;
                return false;
            }
        }

        public static void SetupVanityInventory()
        {
            Plugin.L.LogWarning("Setting up Vanity Item Inventory!");
            PersistentInventoryManager.Current.m_vanityItemsInventory.UpdateItems(CreateVanityPlayerData());
        }

        internal static VanityItemPlayerData CreateVanityPlayerData()
        {
            if (Plugin.simpleProgressionLoaded)
                return SimpleProgressionInterop.GetVanityPlayerData();

            var allBlocks = GameDataBlockBase<VanityItemsTemplateDataBlock>.GetAllBlocks();

            var validBlocks = new List<VanityItemsTemplateDataBlock>();

            foreach (VanityItemsTemplateDataBlock block in allBlocks)
            {
                if (!Unlock.IsAllowedToUnlock(block))
                    continue;

                validBlocks.Add(block);
            }
            
            var vanity = new VanityItemPlayerData(ClassInjector.DerivedConstructorPointer<VanityItemPlayerData>());

            var vanityArray = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DropServer.VanityItems.VanityItem>(validBlocks.Count);

            var c = 0;
            foreach (var block in validBlocks)
            {
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
