using UnityEngine;

public class TreePlacer : IBlockPlacer
{
    private GameObject prefab;

    public TreePlacer(GameObject treePrefab)
    {
        prefab = treePrefab;
    }

    public void Place(Vector3Int gridPos)
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
}