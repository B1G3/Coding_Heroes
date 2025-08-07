using System.Collections.Generic;
using UnityEngine;

public class NodeRegistry : MonoBehaviour
{
    public static NodeRegistry Instance { get; private set; }
    Dictionary<Vector3Int, IGridNode> _nodes = new();

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Register(IGridNode node)
    { 
        _nodes[node.GridPosition] = node;
        Debug.Log($"[NodeRegistry] Registered node: {node.GridPosition}, type: {node.GetType()}");
    }
    
    public void Register(IGridNode node, Vector3Int pos)
    { 
        _nodes[pos] = node;
        Debug.Log($"[NodeRegistry] Registered node: {pos}, type: {node.GetType()}");
    }
    

    public IGridNode GetNodeAt(Vector3Int pos)
        => _nodes.TryGetValue(pos, out var n) ? n : null;
}