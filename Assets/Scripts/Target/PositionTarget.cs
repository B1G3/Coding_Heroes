using UnityEngine;

public class PositionTarget : ITarget
{
    public Vector3 Position { get; private set; }

    public PositionTarget(Vector3 position)
    {
        Position = position;
    }

}