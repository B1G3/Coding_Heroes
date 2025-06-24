using UnityEngine;

public interface IBlockPlacer
{
    void StartPlacement();
    void UpdatePreview(Vector3Int gridPos);
    void ConfirmPlacement(Vector3Int gridPos);
    void CancelPlacement();
    bool IsPlaceable(Vector3Int gridPos);
}
