using GameData;
using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;

namespace AllVanity;

public static class Unlock
{
    private static List<uint> _allowedToUnlock = null!;
    private static readonly HashSet<Func<VanityItemsTemplateDataBlock, UnlockState>> _lockStateFuncs = new();

    public static void RegisterUnlockMethod(Func<VanityItemsTemplateDataBlock, UnlockState> func)
    {
        if (func == null)
            return;
        _lockStateFuncs.Add(func);
    }

    public static bool IsAllowedToUnlock(VanityItemsTemplateDataBlock block)
    {
        if (block == null)
            return false;

        if (_lockStateFuncs.Count == 0)
        {
            if (_allowedToUnlock != null)
            {
                return _allowedToUnlock.Contains(block.persistentID);
            }
            
            if (block.name.StartsWith("LOCK_"))
                return false;

            return true;
        }

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

        if (block.name.StartsWith("LOCK_"))
            return false;

        return !doLock;
    }

    public static void ReloadInventory()
    {
        PersistentInventoryManager.SetInventoryDirty();
    }

    internal static void LoadUnlockFile()
    {
        try
        {
            var path = Path.Combine(Paths.ConfigPath, "AllVanity_UnlockList.json");
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            _allowedToUnlock = System.Text.Json.JsonSerializer.Deserialize<List<uint>>(json);
        }
        catch (Exception ex)
        {
            Plugin.L.LogError($"Error loading unlock file: {ex.GetType().Name}: {ex.Message}");
            Plugin.L.LogWarning($"StackTrace:\n{ex.StackTrace}");
            _allowedToUnlock = null;
        }
    }
}