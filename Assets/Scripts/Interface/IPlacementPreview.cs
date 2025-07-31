using UnityEngine;

public interface IPlacementPreview
{
    void CreatePreview();
    void UpdatePreview(Vector3 gridPos, bool isValid);
    void ClearPreview();
}