using UnityEngine;

public class TreePlacer : IBlockPlacer
{
    private GameObject prefab;
    private GameObject previewInstance;
    private LayerMask ignoredLayers;

    public TreePlacer(GameObject treePrefab, LayerMask treeIgnoredLayers)
    { 
        prefab = treePrefab;
        ignoredLayers = treeIgnoredLayers;
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
        if (IsPlaceable(gridPos))
        {
            var obj = Object.Instantiate(prefab, gridPos, Quaternion.identity);
            var tree = obj.GetComponent<IHarvestable>() as MonoBehaviour;

            if (tree != null && obj.TryGetComponent(out BoxBase box))
            {
                box.Initialize(gridPos);
                FlowManager.Instance.RegisterBox(box);
                Debug.Log($"[Placement] Placed Tree at {gridPos}");
            }
            else
            {
                Debug.LogError("[TreePlacer] Invalid prefab setup.");
                Object.Destroy(obj);
            }
        }

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
            
            // 다른 오브젝트가 겹쳐 있다면 설치 불가
            Debug.Log($"[Tree] {hit.gameObject.name} was placed at {pos}");
            return false;
        }

        // 겹치는 것 없음 or 무시해도 되는 것만 있음 → 설치 가능
        return true;
    }

    private void SetPreviewMaterial(GameObject obj)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            r.material.color = new Color(0, 1, 0, 0.5f);
        }
        obj.AddComponent<BlockPreview>();
    }
}