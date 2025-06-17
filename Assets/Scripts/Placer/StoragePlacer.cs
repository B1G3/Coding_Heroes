using UnityEngine;

public class StoragePlacer : IBlockPlacer
{
    private GameObject prefab;
    private GameObject uiPrefab;
    private GameObject previewInstance;
    private LayerMask ignoredLayers;

    public StoragePlacer(GameObject storagePrefab, GameObject storageUIPrefab, LayerMask ignoredLayers)
    {
        prefab = storagePrefab;
        uiPrefab = storageUIPrefab;
        this.ignoredLayers = ignoredLayers;
    }

    public void StartPlacing()
    {
        previewInstance = Object.Instantiate(prefab);
        SetPreviewMaterial(previewInstance);
    }

    public void UpdatePreview(Vector3Int gridPos)
    {
        previewInstance.transform.position = gridPos;
        bool canPlace = IsPlaceable(gridPos);
        previewInstance.GetComponent<BlockPreview>()?.SetValid(canPlace);
    }

    public void ConfirmPlacement(Vector3Int gridPos)
    {
        if (!IsPlaceable(gridPos))
        {
            CancelPlacing();
            return;
        }

        var obj = Object.Instantiate(prefab, gridPos, Quaternion.identity);
        var box = obj.GetComponent<StorageBox>();
        box.Initialize(gridPos);

        var storageModule = obj.AddComponent<BasicStorage>();
        box.SetStorageModule(storageModule);
        
        var uiObj = Object.Instantiate(uiPrefab, obj.transform);
        uiObj.transform.localPosition = new Vector3(0, 1.5f, 0);
        var ui = uiObj.GetComponent<StorageUI>();
        ui.Initialize(storageModule);

        FlowManager.Instance.RegisterBox(box);
        Debug.Log($"[Placement] Placed StorageBox at {gridPos}");

        CancelPlacing();
    }

    public void CancelPlacing()
    {
        if (previewInstance != null)
            Object.Destroy(previewInstance);
    }

    private bool IsPlaceable(Vector3Int pos)
    {
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.4f);
        foreach (var hit in hits)
        {
            if (((1 << hit.gameObject.layer) & ignoredLayers) != 0)
                continue;
            if (hit.gameObject == previewInstance)
                continue;

            Debug.Log($"[StoragePlacer] Blocked by {hit.gameObject.name} at {pos}");
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
