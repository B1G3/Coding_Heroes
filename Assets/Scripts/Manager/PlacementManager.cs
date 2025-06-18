using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private List<BlockPrefabBinding> blockPrefabs;
    
    private Dictionary<BlockType, IBlockPlacer> placers;
    private BlockType selectedType = BlockType.None;
    
    private bool isPlacing = false;
    private IBlockPlacer activePlacer;
    
    void Awake()
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
            {
                placers[binding.type] = placer;
            }
            else
            {
                Debug.LogWarning($"[PlacementManager] No placer defined for {binding.type}");
            }
        }
    }
    
    private void OnEnable()
    {
        BlockSelection.OnBlockTypeSelected += OnBlockTypeChanged;
    }

    private void OnDisable()
    {
        BlockSelection.OnBlockTypeSelected -= OnBlockTypeChanged;
    }

    private void OnBlockTypeChanged(BlockType type)
    {
        selectedType = type;
        Debug.Log($"[PlacementManager] Ready to place: {type}");
    }

    private void Update()
    {
        if (selectedType == BlockType.None) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, placementMask)) return;

        Vector3Int gridPos = Vector3Int.RoundToInt(hit.point);

        if (Input.GetMouseButtonDown(0))
        {
            if (placers.TryGetValue(selectedType, out var placer))
            {
                isPlacing = true;
                activePlacer = placer;
                activePlacer.StartPlacing();
            }
        }

        if (isPlacing)
        {
            activePlacer.UpdatePreview(gridPos);

            if (Input.GetMouseButtonUp(0))
            {
                activePlacer.ConfirmPlacement(gridPos);
                isPlacing = false;
                activePlacer = null;
                BlockSelection.Clear();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                activePlacer.CancelPlacing();
                isPlacing = false;
                activePlacer = null;
                BlockSelection.Clear();
            }
        }
    }
}