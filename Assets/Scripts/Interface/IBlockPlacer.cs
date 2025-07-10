using UnityEngine;

public interface IBlockPlacer
{
    void StartPlacement(GameObject prefab);
    void UpdatePlacement(Vector3Int gridPos);
    void ConfirmPlacement(Vector3Int gridPos);
    void CancelPlacement();
}
