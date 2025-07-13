using UnityEngine;

public interface ITarget
{
    Vector3 Position { get; }
    Vector3 StartPos { get; set; }
    Vector3 EndPos { get; set; }
}
