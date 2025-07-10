using UnityEngine;

public abstract class IGridNode : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector3Int GridPosition { get; private set; }

    public virtual void Initialize(Vector3Int gridPos)
    {
        GridPosition = gridPos;
    }
}
