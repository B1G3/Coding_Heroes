using UnityEngine;

public abstract class GridNode : MonoBehaviour
{
    public Vector3Int GridPosition { get; private set; }

    public virtual void Initialize(Vector3Int gridPos)
    {
        GridPosition = gridPos;
    }
}
