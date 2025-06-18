using UnityEngine;

public class RailBox : BoxBase
{
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
    
    public RailModule GetRailModule() => railModule;
}
