using UnityEngine;

public class BasicStorage : StorageModule
{
    [SerializeField] private int capacity = 10;
    private int stored = 0;

    public override bool CanStore(int amount)
    {
        return stored + amount <= capacity;
    }

    public override bool TryStore(int amount)
    {
        if (!CanStore(amount)) return false;
        stored += amount;
        
        InvokeStorageChanged(stored, capacity);
        return true;
    }
    
    public override int GetStoredAmount() => stored;
    public override int GetCapacity() => capacity;
}