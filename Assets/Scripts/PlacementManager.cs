using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using static BlockConfig;

public class PlacementManager : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField]
    private LayerMask placementMask;

    [SerializeField]
    private BlockBindings blockBindings;
    
    private Dictionary<BlockType, List<IBlockPlacer>> placerLists;
    
    private BlockType selectedType = BlockType.None;
    private int selectedBlockIndex = -1;
    
    private IBlockPlacer activePlacer;
    private bool isPlacing;
    private Vector3Int cachedGridPos;

    public async UniTask InitializeAsync()
    {
        placerLists = new Dictionary<BlockType, List<IBlockPlacer>>();

        foreach (var entry in blockBindings.entries)
        {
            var list = new List<IBlockPlacer>(entry.prefabs.Count);
            foreach (var prefabEntry in entry.prefabs)
            {
                var placer = new NormalPlacer(prefabEntry.prefab, prefabEntry.ignoredLayers);
                list.Add(placer);
            }
            
            placerLists.Add(entry.type, list);
        }

        BlockSelection.OnBlockTypeSelected += OnBlockTypeChanged;
        InputManager.Instance.PlaceAction.performed   += OnPlaceStarted;
        InputManager.Instance.PlaceAction.canceled    += OnPlaceReleased;
        InputManager.Instance.CancelAction.performed  += OnCancel;

        await UniTask.Yield();
    }

    public void Dispose()
    {
        BlockSelection.OnBlockTypeSelected -= OnBlockTypeChanged;
        InputManager.Instance.PlaceAction.performed   -= OnPlaceStarted;
        InputManager.Instance.PlaceAction.canceled    -= OnPlaceReleased;
        InputManager.Instance.CancelAction.performed  -= OnCancel;
    }

    private void Update()
    {
        if (selectedType == BlockType.None || !isPlacing)
            return;

        var mousePos = Mouse.current.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(mousePos);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, placementMask))
            return;

        cachedGridPos = Vector3Int.RoundToInt(hit.point);
        activePlacer?.UpdatePreview(cachedGridPos);
    }

    private void OnBlockTypeChanged(BlockType type, int index)
    {
        selectedType = type;
        selectedBlockIndex = index;
        Debug.Log($"[PlacementManager] Selected: {type}, {index}");
    }

    private void OnPlaceStarted(InputAction.CallbackContext ctx)
    {
        if (selectedType == BlockType.None || isPlacing) return;
        if (placerLists.TryGetValue(selectedType, out var list)
            && selectedBlockIndex >= 0
            && selectedBlockIndex < list.Count)
        {
            isPlacing = true;
            activePlacer = list[selectedBlockIndex];
            activePlacer.StartPlacement();
        }
    }

    private void OnPlaceReleased(InputAction.CallbackContext ctx)
    {
        if (isPlacing && activePlacer != null)
        {
            activePlacer.ConfirmPlacement(cachedGridPos);
            EndPlacement();
        }
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (isPlacing && activePlacer != null)
        {
            activePlacer.CancelPlacement();
            EndPlacement();
        }
    }

    private void EndPlacement()
    {
        isPlacing    = false;
        activePlacer = null;
        BlockSelection.Clear();
    }
}
