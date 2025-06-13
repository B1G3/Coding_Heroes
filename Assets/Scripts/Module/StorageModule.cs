using System;
using UnityEngine;

public abstract class StorageModule : MonoBehaviour, IStorable
{
    public event Action<int, int> OnStorageChanged; // (current, max)

    protected void InvokeStorageChanged(int current, int max)
    {
        OnStorageChanged?.Invoke(current, max);
    }
    public abstract bool CanStore(int amount);
    public abstract bool TryStore(int amount);
    public abstract int GetStoredAmount();
    public abstract int GetCapacity();
}