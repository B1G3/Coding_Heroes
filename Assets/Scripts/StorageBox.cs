using UnityEngine;

public class StorageBox : BoxBase
{
    [Header("Module Slots")]
    [SerializeField] private StorageModule storageModule;
    
    public void SetStorageModule(StorageModule module) => storageModule = module;
    
}
