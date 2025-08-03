using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }
    void Awake() => Instance = this;
    
    private Dictionary<GameObject, IInput> inputBlocks = new();
    private Dictionary<GameObject, IOutput> outputBlocks = new();
    private Dictionary<Collider, GameObject> colliderToBlock = new();

    public void RegisterBlock(GameObject target)
    {
        if (target.TryGetComponent(out IInput input)) inputBlocks.Add(target, input);
        if (target.TryGetComponent(out IOutput output)) outputBlocks.Add(target, output);
        
        var colliders = target.GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (var col in colliders)
        {
            colliderToBlock[col] = target;
        }
    }

    public void UnregisterBlock(GameObject target)
    {
        inputBlocks.Remove(target);
        outputBlocks.Remove(target);
        
        var colliders = target.GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (var col in colliders)
        {
            if (colliderToBlock.TryGetValue(col, out var existing) && existing == target)
                colliderToBlock.Remove(col);
        }
    }
    
    private GameObject? GetBlockFromCollider(Collider col)
    {
        return colliderToBlock.GetValueOrDefault(col);
    }

    public bool IsInputBlock(Collider targetCollider)
    {
        var target = GetBlockFromCollider(targetCollider);
        return target != null && inputBlocks.ContainsKey(target);
    }
    
    public bool IsOutputBlock(Collider targetCollider)
    {
        var target = GetBlockFromCollider(targetCollider);
        return target != null && outputBlocks.ContainsKey(target);
    }
    
    public IInput? GetInputBlock(Collider targetCollider)
    {
        var target = GetBlockFromCollider(targetCollider);
        return target != null && inputBlocks.TryGetValue(target, out var input) ? input : null;
    }

    public IOutput? GetOutputBlock(Collider targetCollider)
    {
        var target = GetBlockFromCollider(targetCollider);
        return target != null && outputBlocks.TryGetValue(target, out var output) ? output : null;
    }
}
