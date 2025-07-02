using UnityEngine;

public class BlockPlacerService : MonoBehaviour, IBlockPlacer
{
    [SerializeField] LayerMask ignoreMask;

    private ICollisionValidator _validator;
    private IPlacementPreview _preview;
    private NodeRegistry _registry;
    private GameObject _placementPrefab;

    void Awake()
    {
        _validator = new PhysicsCollisionValidator(ignoreMask);
        _preview   = gameObject.AddComponent<PlacementPreview>();
        _registry  = NodeRegistry.Instance;
    }
    
    public void SetCollisionValidator(ICollisionValidator validator)
    {
        _validator = validator;
    }

    public void StartPlacement(GameObject prefab)
    {
        _placementPrefab = prefab;
        _preview.CreatePreview(prefab);
    }

    public void UpdatePlacement(Vector3Int pos)
    {
        bool ok = _validator.CanPlace(pos);
        _preview.UpdatePreview(pos, ok);
    }

    public void ConfirmPlacement(Vector3Int pos)
    {
        if (_validator.CanPlace(pos))
        {
            var go = Instantiate(_placementPrefab, pos, Quaternion.identity);
            if (go.TryGetComponent<IGridNode>(out var node))
                _registry.Register(node);
        }
        _preview.ClearPreview();
    }

    public void CancelPlacement()
        => _preview.ClearPreview();
}