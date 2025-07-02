using UnityEngine;

public class PhysicsCollisionValidator : ICollisionValidator
{
    private LayerMask _ignoreMask;
    public PhysicsCollisionValidator(LayerMask ignoreMask)
        => _ignoreMask = ignoreMask;

    public bool CanPlace(Vector3Int pos)
    {
        var hits = Physics.OverlapBox(
            pos, Vector3.one * 0.4f, Quaternion.identity, ~_ignoreMask);
        return hits.Length == 0;
    }
}