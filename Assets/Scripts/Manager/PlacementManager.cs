using System.Collections.Generic;
using UnityEngine;
using static Const;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private List<BlockPrefabBinding> blockPrefabs;
    
    private Dictionary<BlockType, IBlockPlacer> placers;
    private BlockType selectedType = BlockType.None;
    
    void Awake()
    {
        placers = new Dictionary<BlockType, IBlockPlacer>();

        foreach (var binding in blockPrefabs)
        {
            IBlockPlacer placer = binding.type switch
            {
                BlockType.Tree => new TreePlacer(binding.prefab),
                BlockType.Harvester => new HarvesterPlacer(binding.prefab, binding.uiPrefab),
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
        if (Input.GetMouseButtonDown(0) && selectedType != BlockType.None)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3Int gridPos = Vector3Int.RoundToInt(hit.point);
                PlaceBlock(gridPos);
            }
        }
    }
    
    public void SetSelectedBlockType(BlockType type)
    {
        selectedType = type;
    }
    
    private void PlaceBlock(Vector3Int gridPos)
    {
        if (placers.TryGetValue(selectedType, out var placer))
        {
            placer.Place(gridPos);
        }
        else
        {
            Debug.LogError($"[PlacementManager] No placer found for {selectedType}");
        }

        BlockSelection.Clear();
    }
}