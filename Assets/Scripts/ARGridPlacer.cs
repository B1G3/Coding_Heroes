using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARGridPlacer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Transform origin;     // 기준 Transform (AR에 배치한 빈 오브젝트)
    [SerializeField] private Vector3 cellSize;
    
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;

    // 이미 배치된 칸 관리
    private Dictionary<Vector3Int, GameObject> placedBlocks = new();
    
    
    [SerializeField] private TMP_Text text;
    
    private void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += SetOrigin;
        leftBlockHolder.OnPlaceBlock += PlaceAtWorldPosition;
        rightBlockHolder.OnPlaceBlock += PlaceAtWorldPosition;
    }
    
    private void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= SetOrigin;
        leftBlockHolder.OnPlaceBlock -= PlaceAtWorldPosition;
        rightBlockHolder.OnPlaceBlock -= PlaceAtWorldPosition;
    }

    private void SetOrigin(GameObject origin)
    {
        this.origin = origin.transform;
    }

    /// <summary>
    /// worldPos 위치에 블럭을 배치 시도
    /// </summary>
    public void PlaceAtWorldPosition(Vector3 worldPos, GameObject blockPrefab)
    {
        // 2) cell 인덱스로 변환 (반올림)
        var cell = WorldToCell(worldPos);

        // 이미 배치된 칸이면 무시
        if (placedBlocks.ContainsKey(cell)) return;

        // 3) 그리드 좌표 → world 좌표
        Vector3 cellCenterLocal = new Vector3(
            cell.x * cellSize.x,
            cell.y * cellSize.y,
            cell.z * cellSize.z
        );
        Vector3 spawnPos = origin.TransformPoint(cellCenterLocal);
        spawnPos.y = worldPos.y;

        // 4) Instantiate & 정렬
        var block = Instantiate(blockPrefab, spawnPos, origin.rotation);
        placedBlocks[cell] = block;
        
        text.text = $"Place {blockPrefab.name} at {cell}";
    }

    /// <summary>
    /// worldPos 위치의 칸 정보를 Vector3Int로 조회
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        Vector3 localPos = origin.InverseTransformPoint(worldPos);
        return new Vector3Int(
            Mathf.RoundToInt(localPos.x / cellSize.x),
            Mathf.RoundToInt(localPos.y / cellSize.y),
            Mathf.RoundToInt(localPos.z / cellSize.z)
        );
    }
}