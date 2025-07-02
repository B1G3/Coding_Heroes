using UnityEngine;

public interface ICollisionValidator
{
    bool CanPlace(Vector3Int gridPos);
}