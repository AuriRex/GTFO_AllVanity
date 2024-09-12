using DropServer.VanityItems;
using GameData;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AllVanity.Interop
{
    internal static class SimpleProgressionInterop
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static VanityItemPlayerData GetVanityPlayerData()
        {
            return Impl.Get();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TouchAndAckIds(params uint[] touchIds)
        {
            Impl.TouchAndAckIds(touchIds);
        }

        private static class Impl
        {
            internal static VanityItemPlayerData Get()
            {
                var validLocalItems = SimpleProgression.Core.LocalVanityItemManager.Instance.LocalVanityItemPlayerData.GetValidItemsAndFixCustomIDs();

                var allBlocks = GameDataBlockBase<VanityItemsTemplateDataBlock>.GetAllBlocks();

                var validBlocks = new List<VanityItemsTemplateDataBlock>();

                foreach (VanityItemsTemplateDataBlock block in allBlocks)
                {
                    if (!Unlock.IsAllowedToUnlock(block) && !validLocalItems.Any(i => i.ItemID == block.persistentID))
                        continue;

                    validBlocks.Add(block);
                }

                var vanity = new VanityItemPlayerData(ClassInjector.DerivedConstructorPointer<VanityItemPlayerData>());

                var vanityArray = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DropServer.VanityItems.VanityItem>(validBlocks.Count);

                var c = 0;
                foreach (var block in validBlocks)
                {
                    var localItem = validLocalItems.FirstOrDefault(i => i.ItemID == block.persistentID);

                    DropServer.VanityItems.VanityItem item = new DropServer.VanityItems.VanityItem(ClassInjector.DerivedConstructorPointer<DropServer.VanityItems.VanityItem>())
                    {
                        Flags = localItem == null ? InventoryItemFlags.Touched | InventoryItemFlags.Acknowledged : (InventoryItemFlags)localItem.Flags,
                        ItemId = block.persistentID,
                    };

                    vanityArray[c] = item;
                    c++;
                }

                vanity.Items = vanityArray;

                return vanity;
            }

            internal static void TouchAndAckIds(params uint[] ids)
            {
                var manager = SimpleProgression.Core.LocalVanityItemManager.Instance;
                manager.TouchIds(ids);
                manager.AcknowledgeIds(ids);
            }
        }
    }
}
