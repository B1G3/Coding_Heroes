using System.Collections.Generic;
using UnityEngine;
using static GridUtils;
using static BlockConfig;

public class ARGridPlacer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform origin;
    
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;

    [Header("Prefab")]
    [SerializeField] private GameObject cornerLeftPrefab;
    [SerializeField] private GameObject cornerRightPrefab;
    
    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += SetOrigin;
        leftBlockHolder.OnPlaceBlock += PlaceAtWorldPosition;
        leftBlockHolder.OnPlaceCorner += PlaceFromToWorldPosition;
        rightBlockHolder.OnPlaceBlock += PlaceAtWorldPosition;
        rightBlockHolder.OnPlaceCorner += PlaceFromToWorldPosition;
    }
    
    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= SetOrigin;
        leftBlockHolder.OnPlaceBlock -= PlaceAtWorldPosition;
        leftBlockHolder.OnPlaceCorner -= PlaceFromToWorldPosition;
        rightBlockHolder.OnPlaceBlock -= PlaceAtWorldPosition;
        rightBlockHolder.OnPlaceCorner -= PlaceFromToWorldPosition;
    }

    private void SetOrigin(GameObject home)
    {
        origin = home.transform;
    }

    /// <summary>
    /// worldPos 위치에 블럭을 배치 시도
    /// </summary>
    private void PlaceAtWorldPosition(Vector3 worldPos, GameObject blockPrefab, float rotationY)
    {
        // 2) cell 인덱스로 변환 (반올림)
        var cell = WorldToCell(worldPos);

        // 이미 배치된 칸이면 무시
        if (GridState.Instance.TryGetNode(cell, out _)) return;

        // 3) 그리드 좌표 → world 좌표
        Vector3 spawnPos = CellToWorld(cell);
        spawnPos.y = worldPos.y;
        
        var rotation = GetSpawnRotation(rotationY);

        // 4) Instantiate & 정렬
        var block = Instantiate(blockPrefab, spawnPos, rotation);
        var grid = block.GetComponent<IGridNode>();
        
        // GridState 등록
        GridState.Instance.AddOrUpdateNode(cell, grid);
        
        // BlockManager 등록
        BlockManager.Instance.RegisterBlock(block);
        
        // Initialize 호출 (IfBlock의 경우 내부적으로 서브노드들도 등록됨)
        grid.Initialize(cell, rotationY);
        
        var start = grid as StartNode;
        if (start)
        {
            GameManager.Instance.SaveStartNode(start);
        }
}
    
    // 코너 설치는 이런 식으로 하면 될 듯
    // 근데 두번 째 값에 좌표가 들어오긴 해야함
    private void PlaceFromToWorldPosition(GameObject fromWorldPos, Vector3Int to, GameObject pathPrefab, float rotationY)
    {
        var from = WorldToCell(fromWorldPos.transform.position);
        var fromNode = fromWorldPos.GetComponentInParent<IGridNode>();
        var rotation = GetSpawnRotation(rotationY);
        
        IGridNode toNode;
        
        Quaternion cornerRotation = rotation; // fallback
        if (fromWorldPos.TryGetComponent(out IOutput output))
        {
            WorldDirection outDir = output.GetOutputDirection();
            cornerRotation = RotationFromWorldDirection(outDir);
        }
        
        if (GridState.Instance.TryGetNode(to, out toNode))
        {
            // ▶ 블록이 있으면 블록–블록 연결
            ConnectNode(fromNode, toNode);
        }
        else
        {
            // ▶ 블록이 없으면 코너 스폰 후 딕셔너리에 등록, then 연결
            Vector3 cornerWorldPos = CellToWorld(to);
            // 1) 월드 공간에서의 yaw
            float worldYaw = cornerRotation.eulerAngles.y;
            // 2) origin 기준 yaw
            float originYaw = origin.rotation.eulerAngles.y;
            // 3) 상대 yaw 계산 (–180~180 범위로도, 0~360 범위로도 좋습니다)
            float relativeYaw = (worldYaw - originYaw + 360f) % 360f;
            
            int step = Mathf.RoundToInt(rotationY / 90f);
            bool useLeft = (step % 2) == 0;
            GameObject chosenPrefab = useLeft ? cornerLeftPrefab : cornerRightPrefab;
            
            GameObject cornerGO = Instantiate(chosenPrefab, cornerWorldPos, cornerRotation);
            toNode = cornerGO.GetComponent<IGridNode>();
            toNode.Initialize(to, relativeYaw);
            GridState.Instance.AddOrUpdateNode(to, toNode);
            BlockManager.Instance.RegisterBlock(cornerGO);
        
            ConnectNode(fromNode, toNode);
        }
        
        var dir = to - from;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.z = Mathf.Clamp(dir.z, -1, 1);
        
        if (from + dir == to)
            return;
        
        for (var p = from + dir; ; p += dir)
        {
            Vector3 spawnPos = CellToWorld(p);
            spawnPos.y = fromWorldPos.transform.position.y;
            var path = Instantiate(pathPrefab, spawnPos, cornerRotation);
            var grid = path.GetComponent<IGridNode>();
            GridState.Instance.AddOrUpdateNode(p, grid);
            grid.Initialize(p, rotationY);
            if (p == to - dir) break;
        }
    }
    
    private void ConnectNode(IGridNode fromNode, IGridNode toNode)
    {
        // IfBlock의 경우 적절한 서브노드와 연결
        IConnectable from = GetConnectableOutput(fromNode);
        IConnectable to = GetConnectableInput(toNode);
        
        if (from != null && to != null)
        {
            from.ConnectNext(to);
            to.ConnectPrev(from);
        }
    }
    
    private Quaternion GetSpawnRotation(float rotationY)
    {
        return origin.rotation * Quaternion.Euler(0f, rotationY, 0f);
    }
    
    private IConnectable GetConnectableOutput(IGridNode node)
    {
        // IfBlock의 경우 IfNode가 출력 담당
        if (node is IfBlock ifBlock)
        {
            return ifBlock.GetIfNode() as IConnectable;
        }
        
        return node as IConnectable;
    }

    private IConnectable GetConnectableInput(IGridNode node)
    {
        // IfBlock의 경우 DataCollectorNode가 입력 담당
        if (node is IfBlock ifBlock)
        {
            return ifBlock.GetDataCollectorNode() as IConnectable;
        }
        
        return node as IConnectable;
    }
}