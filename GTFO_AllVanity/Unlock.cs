using DropServer.VanityItems;
using GameData;
using Il2CppInterop.Runtime.Injection;
using System.Collections;
using UnityEngine;

namespace AllVanity
{
    public class Unlock
    {
        public static IEnumerator DelayedSetupVanity()
        {
            EntryPoint.L.LogDebug("Delayed Setup ...");

            yield return new WaitForSeconds(0.5f);

            SetupVanityInventory();
        }

        public static void SetupVanityInventory()
        {
            EntryPoint.L.LogWarning("Setting up Vanity Item Inventory!");
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
