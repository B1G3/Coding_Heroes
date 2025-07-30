using System.Collections.Generic;
using UnityEngine;

public class GridState : MonoBehaviour
{
    public static GridState Instance { get; private set; }

    private Dictionary<Vector3Int, IGridNode> _placedBlocks = new();

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else              Instance = this;
    }

    // 읽기 전용 조회
    public bool TryGetNode(Vector3Int cell, out IGridNode node)
        => _placedBlocks.TryGetValue(cell, out node);

    // 추가/갱신
    public void AddOrUpdateNode(Vector3Int cell, IGridNode node)
        => _placedBlocks[cell] = node;

    // 제거
    public bool RemoveNode(Vector3Int cell)
        => _placedBlocks.Remove(cell);

    // 전체 초기화
    public void ClearAll()
        => _placedBlocks.Clear();
}