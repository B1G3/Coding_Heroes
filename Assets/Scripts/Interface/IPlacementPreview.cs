using UnityEngine;

public interface IPlacementPreview
{
    void CreatePreview(GameObject prefab);
    void UpdatePreview(Vector3Int gridPos, bool isValid);
    void ClearPreview();
}