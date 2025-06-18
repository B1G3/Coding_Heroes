using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static BlockConfig;

public class PlacementManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private List<BlockPrefabBinding> blockPrefabs;
    [SerializeField] private InputActionAsset inputActions;

    private Dictionary<BlockType, IBlockPlacer> placers;
    private BlockType selectedType = BlockType.None;

    private bool isPlacing = false;
    private IBlockPlacer activePlacer;
    private Vector3Int cachedGridPos;

    public async UniTask InitializeAsync()
    {
        placers = new Dictionary<BlockType, IBlockPlacer>();

        foreach (var binding in blockPrefabs)
        {
            IBlockPlacer placer = binding.type switch
            {
                BlockType.Tree => new TreePlacer(binding.prefab, binding.ignoredLayers),
                BlockType.Harvester => new HarvesterPlacer(binding.prefab, binding.uiPrefab, binding.ignoredLayers),
                BlockType.Storage => new StoragePlacer(binding.prefab, binding.uiPrefab, binding.ignoredLayers),
                _ => null
            };

            if (placer != null)
                placers[binding.type] = placer;
            else
                Debug.LogWarning($"[PlacementManager] No placer defined for {binding.type}");
        }

        BlockSelection.OnBlockTypeSelected += OnBlockTypeChanged;
        InputManager.Instance.PlaceAction.performed += OnPlaceStarted;
        InputManager.Instance.PlaceAction.canceled += OnPlaceReleased;
        InputManager.Instance.CancelAction.performed += OnCancel;

        await UniTask.Yield();
    }

    public void Dispose()
    {
        BlockSelection.OnBlockTypeSelected -= OnBlockTypeChanged;
        InputManager.Instance.PlaceAction.performed -= OnPlaceStarted;
        InputManager.Instance.PlaceAction.canceled -= OnPlaceReleased;
        InputManager.Instance.CancelAction.performed -= OnCancel;
    }

    private void OnBlockTypeChanged(BlockType type)
    {
        selectedType = type;
        Debug.Log($"[PlacementManager] Ready to place: {type}");
    }

    private void Update()
    {
        if (selectedType == BlockType.None)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, placementMask))
            return;

        cachedGridPos = Vector3Int.RoundToInt(hit.point);

        if (isPlacing && activePlacer != null)
        {
            activePlacer.UpdatePreview(cachedGridPos);
        }
    }

    private void OnPlaceStarted(InputAction.CallbackContext ctx)
    {
        if (selectedType == BlockType.None) return;
    
        if (!isPlacing && placers.TryGetValue(selectedType, out var placer))
        {
            isPlacing = true;
            activePlacer = placer;
            activePlacer.StartPlacing();
        }
    }

    private void OnPlaceReleased(InputAction.CallbackContext ctx)
    {
        if (isPlacing && activePlacer != null)
        {
            activePlacer.ConfirmPlacement(cachedGridPos);
            isPlacing = false;
            activePlacer = null;
            BlockSelection.Clear();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (isPlacing && activePlacer != null)
        {
            activePlacer.CancelPlacing();
            isPlacing = false;
            activePlacer = null;
            BlockSelection.Clear();
        }
    }
}
