using UnityEngine;

public class RailBox : BoxBase
{
    [Header("Module Slots")]
    [SerializeField] private RailModule railModule;

    public void SetRailModule(RailModule module) => railModule = module;
    
    public override void Initialize(Vector3Int gridPosition)
    {
        base.Initialize(gridPosition);
        FlowManager.Instance.RegisterBox(this);
    }

    public override void Tick()
    {
        railModule.Tick(GridPosition);
    }
    
    public override T GetIoModule<T>()
    {
        if (typeof(T) == typeof(RailModule) ||
            typeof(T) == typeof(IInput) ||
            typeof(T) == typeof(IOutput))
        {
            return railModule as T;
        }

        return base.GetIoModule<T>();
    }
}
