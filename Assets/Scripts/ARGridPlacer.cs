using System.Collections.Generic;
using UnityEngine;
using static GridUtils;

public class ARGridPlacer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform origin;
    
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;

    [Header("Prefab")]
    [SerializeField] private GameObject cornerPrefab;
    
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
    private void PlaceAtWorldPosition(Vector3 worldPos, GameObject blockPrefab)
    {
        // 2) cell 인덱스로 변환 (반올림)
        var cell = WorldToCell(worldPos);

        // 이미 배치된 칸이면 무시
        if (GridState.Instance.TryGetNode(cell, out _)) return;

        // 3) 그리드 좌표 → world 좌표
        Vector3 spawnPos = CellToWorld(cell);
        spawnPos.y = worldPos.y;

        // 4) Instantiate & 정렬
        var block = Instantiate(blockPrefab, spawnPos, origin.rotation);
        var grid = block.GetComponent<IGridNode>();
        GridState.Instance.AddOrUpdateNode(cell, grid);
        grid.Initialize(cell);
        
        var start = grid as StartNode;
        if (start)
        {
            GameManager.Instance.SaveStartNode(start);
        }
        
    }
    
    // 코너 설치는 이런 식으로 하면 될 듯
    // 근데 두번 째 값에 좌표가 들어오긴 해야함
    private void PlaceFromToWorldPosition(GameObject fromWorldPos, Vector3Int to, GameObject pathPrefab)
    {
        var from = WorldToCell(fromWorldPos.transform.position);
        var fromNode = fromWorldPos.GetComponentInParent<IGridNode>();
        
        IGridNode toNode;
        
        if (GridState.Instance.TryGetNode(to, out toNode))
        {
            // ▶ 블록이 있으면 블록–블록 연결
            ConnectNode(fromNode, toNode);
        }
        else
        {
            // ▶ 블록이 없으면 코너 스폰 후 딕셔너리에 등록, then 연결
            Vector3 cornerWorldPos = CellToWorld(to);
            GameObject cornerGO = Instantiate(cornerPrefab, cornerWorldPos, origin.rotation);
            toNode = cornerGO.GetComponent<IGridNode>();
            toNode.Initialize(to);
            GridState.Instance.AddOrUpdateNode(to, toNode);
        
            ConnectNode(fromNode, toNode);
        }
        
        var dir = to - from;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.z = Mathf.Clamp(dir.z, -1, 1);
        
        for (var p = from + dir; ; p += dir)
        {
            Vector3 spawnPos = CellToWorld(p);
            spawnPos.y = fromWorldPos.transform.position.y;
            var path = Instantiate(pathPrefab, spawnPos, origin.rotation);
            var grid = path.GetComponent<IGridNode>();
            GridState.Instance.AddOrUpdateNode(p, grid);
            grid.Initialize(p);
            if (p == to - dir) break;
        }
    }
    
    private void ConnectNode(IGridNode fromNode, IGridNode toNode)
    {
        if (fromNode is IConnectable from && toNode is IConnectable to)
        {
            from?.ConnectNext(to);
            to?.ConnectPrev(from);
        }
    }
}