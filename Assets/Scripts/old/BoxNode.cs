using System;
using UnityEngine;

public class BoxNode : GridNode
{
    [SerializeField] private ControlModule controlModule;
    public ControlModule ControlModule => controlModule;

    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        FlowManager.Instance.Register(this);
    }
}
