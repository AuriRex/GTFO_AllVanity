namespace AllVanity
{
    public enum UnlockState
    {
        /// <summary>Skip this block, neither lock nor unlock this item.</summary>
        Skip,

        /// <summary>Try to unlock this item, takes precedence over <see cref="TryLock"/> andis overwritten by both <see cref="ForceUnlock"/> and <see cref="ForceLock"/></summary>
        TryUnlock,

        /// <summary>Try to unlock this item, is overwritten by <see cref="TryUnlock"/>, <see cref="ForceUnlock"/> and <see cref="ForceLock"/></summary>
        TryLock,

        /// <summary>Forces this item to be unlocked, takes precedence over <see cref="ForceLock"/></summary>
        ForceUnlock,

        /// <summary>Forces this item to be locked, unless unlocked by <see cref="ForceUnlock"/></summary>
        ForceLock,
    }
}
