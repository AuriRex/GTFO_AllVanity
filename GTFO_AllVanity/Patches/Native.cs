using BepInEx.Unity.IL2CPP.Hook;
using DropServer.VanityItems;
using GameData;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;

namespace AllVanity.Patches
{
    internal class Native
    {
        public static unsafe void* GetIl2CppMethod<T>(string methodName, string returnTypeName, bool isGeneric, params string[] argTypes) where T : Il2CppObjectBase
        {
            void** ppMethod = (void**)IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<T>.NativeClassPtr, isGeneric, methodName, returnTypeName, argTypes).ToPointer();
            if ((long)ppMethod == 0)
                return ppMethod;
            return *ppMethod;
        }

        private static readonly List<INativeDetour> _detours = new();
        internal static unsafe void ApplyNative()
        {
            _detours.Add(INativeDetour.CreateAndApply((nint)GetIl2CppMethod<VanityItemInventory>(nameof(VanityItemInventory.UpdateItems), "System.Void", false, nameof(VanityItemPlayerData)), UpdateItemsPatch, out _originalUpdateItems));
        }

        private static unsafe UpdateItems _originalUpdateItems;

        public unsafe delegate void UpdateItems(IntPtr _this, IntPtr data, Il2CppMethodInfo* methodInfo);

        public static unsafe void UpdateItemsPatch(IntPtr self, IntPtr vanityItemPlayerData, Il2CppMethodInfo* methodInfo)
        {
            VanityItemInventory __instance = new VanityItemInventory(self);

            _originalUpdateItems.Invoke(self, vanityItemPlayerData, methodInfo);

            var backedIds = new List<uint>();

            if (__instance.m_backednItems == null)
            {
                __instance.m_backednItems = new Il2CppSystem.Collections.Generic.List<VanityItem>(0);
            }

            foreach (VanityItem item in __instance.m_backednItems)
            {
                backedIds.Add(item.id);
            }

            foreach (VanityItemsTemplateDataBlock block in GameDataBlockBase<VanityItemsTemplateDataBlock>.GetAllBlocks())
            {
                if (block == null)
                    continue;

                if (backedIds.Contains(block.persistentID))
                    continue;

                if (!Unlock.IsAllowedToUnlock(block))
                    continue;

                VanityItem item = new VanityItem(ClassInjector.DerivedConstructorPointer<VanityItem>());
                item.publicName = $"<#{Plugin.hexColorUnlocked}>{block.publicName}</color>";
                item.type = block.type;
                item.prefab = block.prefab;
                item.flags = VanityItemFlags.Touched | VanityItemFlags.Acknowledged;
                item.id = block.persistentID;

                __instance.m_backednItems.Add(item);
            }
        }
    }
}
