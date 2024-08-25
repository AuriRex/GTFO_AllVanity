using GameData;
using System;
using System.Collections.Generic;

namespace AllVanity
{
    public static class Unlock
    {
        private static readonly HashSet<Func<VanityItemsTemplateDataBlock, UnlockState>> _lockStateFuncs = new();

        public static void RegisterUnlockMethod(Func<VanityItemsTemplateDataBlock, UnlockState> func)
        {
            if (func == null)
                return;
            _lockStateFuncs.Add(func);
        }

        public static bool IsAllowedToUnlock(VanityItemsTemplateDataBlock block)
        {
            if (_lockStateFuncs.Count == 0)
                return true;

            bool doUnlock = false;
            bool doLock = false;
            bool forceLock = false;
            foreach(var func in _lockStateFuncs)
            {
                try
                {
                    var state = func.Invoke(block);

                    switch (state)
                    {
                        default:
                        case UnlockState.Skip:
                            break;
                        case UnlockState.TryUnlock:
                            doUnlock = true;
                            break;
                        case UnlockState.TryLock:
                            doLock = true;
                            break;
                        case UnlockState.ForceLock:
                            forceLock = true;
                            break;
                        case UnlockState.ForceUnlock:
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    Plugin.L.LogWarning($"A provided {nameof(IsAllowedToUnlock)} func failed!");
                    Plugin.L.LogError($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }

            if (forceLock)
                return false;

            if (doUnlock)
                return true;

            return !doLock;
        }

        public static void ReloadInventory()
        {
            PersistentInventoryManager.SetInventoryDirty();
        }
    }
}
