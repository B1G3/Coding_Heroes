using UnityEngine;

public class HarvesterBox : BoxBase
{
    [Header("Module Slots")]
    [SerializeField] private HarvestModule harvestModule;
    [SerializeField] private StorageModule storageModule;

    public void SetHarvestModule(HarvestModule module) => harvestModule = module;
    public void SetStorageModule(StorageModule module) => storageModule = module;

    public override void Tick()
    {
        if (harvestModule == null || storageModule == null) return;

        if (harvestModule.TryHarvest(out int amount))
        {
            storageModule.TryStore(amount);
        }
    }
}
