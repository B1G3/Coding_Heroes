using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BlockConfig;

public class ARGridPlacer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Transform origin;     // 기준 Transform (AR에 배치한 빈 오브젝트)
    [SerializeField] private Vector3 cellSize;
    
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;

    // 이미 배치된 칸 관리
    private Dictionary<Vector3Int, IGridNode> placedBlocks = new();
    
    
    [SerializeField] private TMP_Text text;
    
    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += SetOrigin;
        leftBlockHolder.OnPlaceBlock += PlaceAtWorldPosition;
        leftBlockHolder.OnPlacePath += PlaceFromToWorldPosition;
        rightBlockHolder.OnPlaceBlock += PlaceAtWorldPosition;
        rightBlockHolder.OnPlacePath += PlaceFromToWorldPosition;
    }
    
    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= SetOrigin;
        leftBlockHolder.OnPlaceBlock -= PlaceAtWorldPosition;
        leftBlockHolder.OnPlacePath -= PlaceFromToWorldPosition;
        rightBlockHolder.OnPlaceBlock -= PlaceAtWorldPosition;
        rightBlockHolder.OnPlacePath -= PlaceFromToWorldPosition;
    }

    private void SetOrigin(GameObject origin)
    {
        this.origin = origin.transform;
    }

    /// <summary>
    /// worldPos 위치에 블럭을 배치 시도
    /// </summary>
    private void PlaceAtWorldPosition(Vector3 worldPos, GameObject blockPrefab)
    {
        // 2) cell 인덱스로 변환 (반올림)
        var cell = WorldToCell(worldPos);

        // 이미 배치된 칸이면 무시
        if (placedBlocks.ContainsKey(cell)) return;

        // 3) 그리드 좌표 → world 좌표
        Vector3 spawnPos = CellToWorld(cell);
        spawnPos.y = worldPos.y;

        // 4) Instantiate & 정렬
        var block = Instantiate(blockPrefab, spawnPos, origin.rotation);
        var grid = block.GetComponent<IGridNode>();
        placedBlocks[cell] = grid;
        grid.Initialize(cell);
        
        text.text = $"Place {blockPrefab.name} at {cell}";
        
        var start = grid as StartNode;
        if (start)
        {
            text.text = $"Place startnode at {cell}";
            GameManager.Instance.SaveStartNode(start);
        }
        
    }

    private void PlaceFromToWorldPosition(GameObject fromWorldPos, GameObject toWorldPos, GameObject pathPrefab)
    {
        var fromNode = fromWorldPos.GetComponentInParent<IGridNode>();
        var toNode = toWorldPos.GetComponentInParent<IGridNode>();
        var from = WorldToCell(fromWorldPos.transform.position);
        var to = WorldToCell(toWorldPos.transform.position);
        
        ConnectNode(fromNode, toNode);
        
        var dir = to - from;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.z = Mathf.Clamp(dir.z, -1, 1);
        
        for (var p = from + dir; ; p += dir)
        {
            Vector3 spawnPos = CellToWorld(p);
            spawnPos.y = fromWorldPos.transform.position.y;
            var path = Instantiate(pathPrefab, spawnPos, origin.rotation);
            var grid = path.GetComponent<IGridNode>();
            placedBlocks[p] = grid;
            grid.Initialize(p);
            if (p == to - dir) break;
        }
    }
    // 아직 꺽는 기능이 없음

    
    /// <summary>
    /// worldPos 위치의 칸 정보를 Vector3Int로 조회
    /// </summary>
    private Vector3Int WorldToCell(Vector3 worldPos)
    {
        Vector3 localPos = origin.InverseTransformPoint(worldPos);
        return new Vector3Int(
            Mathf.RoundToInt(localPos.x / cellSize.x),
            Mathf.RoundToInt(localPos.y / cellSize.y),
            Mathf.RoundToInt(localPos.z / cellSize.z)
        );
    }

    private Vector3 CellToWorld(Vector3Int cell)
    {
        Vector3 cellCenterLocal = new Vector3(
            cell.x * cellSize.x,
            cell.y * cellSize.y,
            cell.z * cellSize.z
        );
        Vector3 spawnPos = origin.TransformPoint(cellCenterLocal);
        return spawnPos;
    }
    
    private void ConnectNode(IGridNode fromNode, IGridNode toNode)
    {
        var from = fromNode as IConnectable;
        var to   = toNode   as IConnectable;
        
        if (from != null && to != null)
        {
            text.text = $"Connect {from} to {to}";
            from?.ConnectNext(to);
            to?.ConnectPrev(from);
        }
    }
}