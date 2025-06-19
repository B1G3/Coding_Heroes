using UnityEngine;
using static BlockConfig;

public class RailPlacer : IBlockPlacer
{
    private GameObject prefab;
    private GameObject uiPrefab;
    private LayerMask ignoredLayers;
    private GameObject previewInstance;

    public RailPlacer(GameObject prefab, GameObject uiPrefab, LayerMask ignoredLayers)
    {
        this.prefab = prefab;
        this.uiPrefab = uiPrefab;
        this.ignoredLayers = ignoredLayers;
    }

    public void StartPlacing()
    {
        previewInstance = Object.Instantiate(prefab);
        // SetPreviewMaterial(previewInstance);
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
        var rail = obj.GetComponent<RailBox>();
        rail.Initialize(gridPos);
        
        var railModule = obj.AddComponent<RailModule>();
        rail.SetRailModule(railModule);
        
        railModule.SetInputDirection(Direction.None);
        railModule.SetOutputDirection(Direction.None);

        var uiObj = Object.Instantiate(uiPrefab, obj.transform);
        uiObj.transform.localPosition = new Vector3(0, 0.25f, 0);
        var ui = uiObj.GetComponent<RailUI>();
        ui.Initialize(railModule);

        FlowManager.Instance.RegisterBox(rail);
        
        AutoConfigureDirection(gridPos, railModule);
        
        // TryConnectWithNeighbors(gridPos, rail);

        Debug.Log($"[Placement] Placed Rail at {gridPos}");
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

            Debug.Log($"[RailPlacer] Blocked by {hit.gameObject.name} at {pos}");
            return false;
        }
        return true;
    }

    // private void SetPreviewMaterial(GameObject obj)
    // {
    //     foreach (var r in obj.GetComponentsInChildren<Renderer>())
    //     {
    //         r.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
    //         r.material.color = new Color(0, 1, 1, 0.5f); // 청록색 반투명
    //     }
    //     obj.AddComponent<BlockPreview>();
    // }
    
    private void AutoConfigureDirection(Vector3Int gridPos, RailModule railModule)
    {
        Direction? inputDir  = null;
        Direction? outputDir = null;
        
        foreach (var dir in DirectionExtensions.AllDirections())
        {
            var neighborPos = gridPos + dir.ToVector();
            var neighbor = FlowManager.Instance.FindBox(neighborPos);
            if (neighbor == null) continue;
            
            
            var neighborOutput = neighbor.GetIoModule<IOutput>();
            if (neighborOutput != null && 
                inputDir == null &&
                neighborOutput.GetOutputDirection() == Direction.None)
            {
                Debug.Log($"1. neighbor: {neighbor}, dir: {dir}");
                neighborOutput.SetOutputDirection(dir.Opposite());
                inputDir = dir;
                continue;
            }
            
            if (neighborOutput != null && neighborOutput.CanSend(dir.Opposite()))
            {
                Debug.Log($"2. neighbor: {neighbor}, dir: {dir}");
                inputDir = dir;
                continue;
            }
            
            var neighborInput = neighbor.GetIoModule<IInput>();
            if (neighborInput != null && 
                outputDir == null &&
                neighborInput.GetInputDirection() == Direction.None)
            {
                Debug.Log($"3. neighbor: {neighbor}, dir: {dir}");
                neighborInput.SetInputDirection(dir.Opposite());
                outputDir = dir;
                continue;
            }
            
            if (neighborInput != null && neighborInput.CanReceive(dir.Opposite()))
            {
                Debug.Log($"4. neighbor: {neighbor}, dir: {dir}");
                outputDir = dir;
            }
        }
        
        railModule.SetInputDirection (inputDir  ?? Direction.None);
        railModule.SetOutputDirection(outputDir ?? Direction.None);
    }
    
    // private void TryConnectWithNeighbors(Vector3Int gridPos, BoxBase rail)
    // {
    //     foreach (var dir in DirectionExtensions.AllDirections())
    //     {
    //         var neighborPos = gridPos + dir.ToVector();
    //         var neighbor = FlowManager.Instance.FindBox(neighborPos);
    //         if (neighbor == null) continue;
    //
    //         ConnectionUtil.TryConnect(rail, neighbor, dir);
    //         ConnectionUtil.TryConnect(neighbor, rail, dir);
    //     }
    // }
}