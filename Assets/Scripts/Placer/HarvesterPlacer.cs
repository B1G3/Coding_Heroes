using UnityEngine;

public class HarvesterPlacer : IBlockPlacer
{
    private GameObject prefab;
    private GameObject uiPrefab;

    public HarvesterPlacer(GameObject harvesterPrefab, GameObject harvesterUIPrefab)
    {
        prefab = harvesterPrefab;
        uiPrefab = harvesterUIPrefab;
    }

    public void Place(Vector3Int gridPos)
    {
        var obj = Object.Instantiate(prefab, gridPos, Quaternion.identity);
        var harvester = obj.GetComponent<HarvesterBox>();
        harvester.Initialize(gridPos);

        var target = FlowManager.Instance.FindNearbyInterface<IHarvestable>(gridPos);
        if (target == null)
        {
            Debug.LogWarning($"[HarvesterPlacer] No IHarvestable nearby. Harvester not placed.");
            Object.Destroy(obj);
            return;
        }

        var harvestModule = obj.AddComponent<TreeHarvester>();
        harvestModule.SetHarvestInterval(2f);
        harvestModule.InitializeTarget(target);
        harvester.SetHarvestModule(harvestModule);

        var storageModule = obj.AddComponent<BasicStorage>();
        harvester.SetStorageModule(storageModule);

        var uiObj = Object.Instantiate(uiPrefab, obj.transform);
        uiObj.transform.localPosition = new Vector3(0, 1.5f, 0);
        var ui = uiObj.GetComponent<HarvesterUI>();
        ui.Initialize(storageModule);

        FlowManager.Instance.RegisterBox(harvester);
        Debug.Log($"[Placement] Placed Harvester at {gridPos} with auto modules");
    }
}