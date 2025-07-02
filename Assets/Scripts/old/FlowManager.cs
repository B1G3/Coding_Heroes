using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance { get; private set; }
    private Dictionary<Vector3Int, GridNode> grid = new();

    void Awake() => Instance = this;

    public void Register(GridNode node)
        => grid[node.GridPosition] = node;

    // 셀 하나하나는 BoxNode거나 RailNode거나
    public GridNode GetNodeAt(Vector3Int pos)
        => grid.TryGetValue(pos, out var n) ? n : null;
}