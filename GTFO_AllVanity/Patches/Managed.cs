using DropServer.VanityItems;
using GameData;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AllVanity.Patches;

internal static class Managed
{
    [HarmonyPatch(typeof(PersistentInventoryManager), nameof(PersistentInventoryManager.CommitPendingTransactions))]
    internal static class PersistentInventoryManager__CommitPendingTransactions__Patch
    {
        [HarmonyPriority(Priority.HigherThanNormal)]
        public static bool Prefix()
        {
            if (!PersistentInventoryManager.m_dirty)
                return true;
            
            SetupVanityInventory();
            PersistentInventoryManager.m_dirty = false;
            return false;
        }
    }

    public static void SetupVanityInventory()
    {
        Plugin.L.LogWarning("Setting up Vanity Item Inventory!");
        PersistentInventoryManager.Current.m_vanityItemsInventory.UpdateItems(CreateVanityPlayerData());
    }

    private static VanityItemPlayerData CreateVanityPlayerData()
    {
        var allBlocks = GameDataBlockBase<VanityItemsTemplateDataBlock>.GetAllBlocks();

        var validBlocks = new List<VanityItemsTemplateDataBlock>();

        foreach (var block in allBlocks)
        {
            if (!Unlock.IsAllowedToUnlock(block))
                continue;

            validBlocks.Add(block);
        }
            
        var vanity = new VanityItemPlayerData(ClassInjector.DerivedConstructorPointer<VanityItemPlayerData>());

        var vanityArray = new Il2CppReferenceArray<DropServer.VanityItems.VanityItem>(validBlocks.Count);

        var c = 0;
        foreach (var block in validBlocks)
        {
            var item = new DropServer.VanityItems.VanityItem(ClassInjector.DerivedConstructorPointer<DropServer.VanityItems.VanityItem>())
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