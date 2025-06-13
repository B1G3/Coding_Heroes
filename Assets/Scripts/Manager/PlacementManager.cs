using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private GameObject harvesterPrefab;
    [SerializeField] private GameObject harvesterUIPrefab;
    
    private int boxCounter = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3Int gridPos = Vector3Int.RoundToInt(hit.point);

                // 예시: Shift 키를 누르면 나무, 안 누르면 수확기
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PlaceTree(gridPos);
                }
                else
                {
                    PlaceHarvester(gridPos);
                }
            }
        }
    }

    void PlaceTree(Vector3Int gridPos)
    {
        var obj = Instantiate(treePrefab, gridPos, Quaternion.identity);
        var tree = obj.GetComponent<IHarvestable>() as MonoBehaviour;

        if (tree != null)
        {
            if (obj.TryGetComponent(out BoxBase box))
            {
                box.Initialize(gridPos);
                FlowManager.Instance.RegisterBox(box);
                Debug.Log($"[Placement] Placed Tree at {gridPos}");
            }
            else
            {
                Debug.LogError("[Placement] Tree prefab lacks BoxBase.");
                Destroy(obj);
            }
        }
        else
        {
            Debug.LogError("[Placement] Tree prefab does not implement IHarvestable.");
            Destroy(obj);
        }
    }

    void PlaceHarvester(Vector3Int gridPos)
    {
        var obj = Instantiate(harvesterPrefab, gridPos, Quaternion.identity);
        var harvester = obj.GetComponent<HarvesterBox>();
        harvester.Initialize(gridPos);

        var target = FlowManager.Instance.FindNearbyInterface<IHarvestable>(gridPos);
        if (target == null)
        {
            Debug.LogWarning($"[Placement] No IHarvestable nearby. Harvester not placed.");
            Destroy(obj);
            return;
        }

        var harvestModule = obj.AddComponent<TreeHarvester>();
        harvestModule.SetHarvestInterval(2f);
        harvestModule.InitializeTarget(target);
        harvester.SetHarvestModule(harvestModule);

        var storageModule = obj.AddComponent<BasicStorage>();
        harvester.SetStorageModule(storageModule);
        
        var uiObj = Instantiate(harvesterUIPrefab, obj.transform);
        uiObj.transform.localPosition = new Vector3(0, 1.5f, 0);
        var ui = uiObj.GetComponent<HarvesterUI>();
        ui.Initialize(storageModule);

        FlowManager.Instance.RegisterBox(harvester);
        Debug.Log($"[Placement] Placed Harvester at {gridPos} with auto modules");
    }

}