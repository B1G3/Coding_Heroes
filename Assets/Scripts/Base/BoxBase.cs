using UnityEngine;

public abstract class BoxBase : MonoBehaviour
{
    public Vector3Int GridPosition { get; private set; }

    public virtual void Initialize(Vector3Int gridPosition) {
        GridPosition = gridPosition;
    }

    public virtual void Tick() { }

    public virtual T GetIoModule<T>() where T : class => null;
}
