using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public abstract class IGridNode : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector3Int GridPosition { get; private set; }

    public virtual void Initialize(Vector3Int gridPos)
    {
        GridPosition = gridPos;
    }
    
    // Input 인터페이스 체크
    public bool HasInput()
    {
        return this is IInput;
    }
    
    // Output 인터페이스 체크  
    public bool HasOutput()
    {
        return this is IOutput;
    }
}