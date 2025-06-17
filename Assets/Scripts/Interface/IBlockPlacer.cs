using UnityEngine;

public interface IBlockPlacer
{
    void StartPlacing();
    void UpdatePreview(Vector3Int gridPos);
    void CancelPlacing();
    void ConfirmPlacement(Vector3Int gridPos);
}