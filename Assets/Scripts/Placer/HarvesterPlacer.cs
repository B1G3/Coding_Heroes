using UnityEngine;

public class HarvesterPlacer : IBlockPlacer
{
    private GameObject prefab;
    private GameObject uiPrefab;
    private GameObject previewInstance;
    private LayerMask ignoredLayers;
    
    private IHarvestable cachedTarget;

    public HarvesterPlacer(GameObject harvesterPrefab, GameObject harvesterUIPrefab, LayerMask harvesterIgnoredLayers)
    {
        prefab = harvesterPrefab;
        uiPrefab = harvesterUIPrefab;
        ignoredLayers = harvesterIgnoredLayers;
    }

    public void StartPlacing()
    {
        previewInstance = Object.Instantiate(prefab);
        SetPreviewMaterial(previewInstance);
    }

    public void UpdatePreview(Vector3Int gridPos)
    {
        previewInstance.transform.position = gridPos;
        bool canPlace = IsPlaceable(gridPos, out cachedTarget);
        previewInstance.GetComponent<BlockPreview>()?.SetValid(canPlace);
    }

    public void ConfirmPlacement(Vector3Int gridPos)
    {
        if (cachedTarget == null)
        {
            CancelPlacing();
            return;
        }

        var obj = Object.Instantiate(prefab, gridPos, Quaternion.identity);
        var harvester = obj.GetComponent<HarvesterBox>();
        harvester.Initialize(gridPos);

        var harvestModule = obj.AddComponent<TreeHarvester>();
        harvestModule.SetHarvestInterval(2f);
        harvestModule.InitializeTarget(cachedTarget);
        harvester.SetHarvestModule(harvestModule);

        var storageModule = obj.AddComponent<BasicStorage>();
        harvester.SetStorageModule(storageModule);

        var uiObj = Object.Instantiate(uiPrefab, obj.transform);
        uiObj.transform.localPosition = new Vector3(0, 1.5f, 0);
        var ui = uiObj.GetComponent<StorageUI>();
        ui.Initialize(storageModule);

        FlowManager.Instance.RegisterBox(harvester);
        Debug.Log($"[Placement] Placed Harvester at {gridPos} with auto modules");

        CancelPlacing();
    }

    public void CancelPlacing()
    {
        if (previewInstance != null)
            Object.Destroy(previewInstance);
    }

    private bool IsPlaceable(Vector3Int pos, out IHarvestable nearbyHarvestable)
    {
        nearbyHarvestable = FlowManager.Instance.FindNearbyInterface<IHarvestable>(pos);
        if (nearbyHarvestable == null)
            return false;
        
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.4f);
        foreach (var hit in hits)
        {
            if (((1 << hit.gameObject.layer) & ignoredLayers) != 0)
                continue;
            if (hit.gameObject == previewInstance) 
                continue;
            
            Debug.Log($"[HarvesterPlacer] Blocked by {hit.gameObject.name} at {pos}");
            return false;
        }
        
        return true;
    }

    private void SetPreviewMaterial(GameObject obj)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            r.material.color = new Color(1, 1, 0, 0.5f);
        }
        obj.AddComponent<BlockPreview>();
    }
}