using UnityEngine;

public class RailNode : GridNode
{
    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        FlowManager.Instance.Register(this);
    }
}
