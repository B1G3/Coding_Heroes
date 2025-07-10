using UnityEngine;

public class PlacementPreview : MonoBehaviour, IPlacementPreview
{
    private GameObject _preview;

    public void CreatePreview(GameObject prefab)
    {
        _preview = GameObject.Instantiate(prefab);
        foreach (var col in _preview.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    public void UpdatePreview(Vector3Int pos, bool isValid)
    {
        _preview.transform.position = pos;
        _preview.GetComponent<BlockPreview>().SetValid(isValid);
    }

    public void ClearPreview()
    {
        if (_preview != null) Destroy(_preview);
        _preview = null;
    }
}