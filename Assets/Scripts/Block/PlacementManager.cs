using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static BlockConfig;

/// <summary>
/// UI 버튼으로 ScriptableObject BlockBindings에 정의된 BlockTypeBinding과 PrefabEntry를 선택,
/// InputManager 이벤트 및 MousePosition을 활용해 Grid 상에 블록/길을 배치합니다.
/// </summary>
public class PlacementManager : MonoBehaviour
{
    [SerializeField] private BlockPlacerService placer;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask blockMask;
    [SerializeField] private BlockBindings blockBindings;
    [SerializeField] private NodeRegistry registry;
    private enum Mode { None, Block, WaitingForPathStart, PlacingPath }
    private Mode mode = Mode.None;
    
    private BlockType currentType = BlockType.None;
    private PrefabEntry currentEntry;
    private Vector3Int gridPos;
    
    private Vector3Int pathStart;

    private void OnEnable()
    {
        InputManager.Instance.PlaceAction.performed += OnPlacePerformed;
        InputManager.Instance.CancelAction.performed += OnCancelPerformed;
        BlockSelection.OnBlockTypeSelected += OnSelectType;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.PlaceAction.performed -= OnPlacePerformed;
            InputManager.Instance.CancelAction.performed -= OnCancelPerformed;
        }
        BlockSelection.OnBlockTypeSelected -= OnSelectType;
    }

    private void Update()
    {
        if (currentType == BlockType.None)
            return;

        if (TryGetGridPosition(out gridPos))
        {
            if (mode == Mode.Block)
            {
                placer.UpdatePlacement(gridPos);
            }
            else if (mode == Mode.PlacingPath)
            {
                var aligned = AlignOnAxis(pathStart, gridPos);
                placer.UpdatePlacement(aligned);
            }
        }
    }

    /// <summary>
    /// UI 버튼 호출: entryIndex는 blockBindings.entries 리스트의 인덱스,
    /// prefabIndex는 해당 entry.prefabs 리스트의 인덱스
    /// </summary>
    private void OnSelectType(BlockType type, int prefabIndex)
    {
        Debug.Log($"[PlacementManager] Selected block type: {type}");
        int entryIndex = (int)type;
        if (blockBindings == null || entryIndex < 0 || entryIndex >= blockBindings.entries.Count)
            return;

        var binding = blockBindings.entries[entryIndex];
        if (binding.prefabs == null || binding.prefabs.Count == 0)
            return;

        if (prefabIndex < 0 || prefabIndex >= binding.prefabs.Count)
            prefabIndex = 0;

        currentType = binding.type;
        currentEntry = binding.prefabs[prefabIndex];
            Debug.Log($"[PlacementManager] Selected block: {currentEntry.prefab.name}");

        // 충돌 검사 설정 갱신
        placer.SetCollisionValidator(new PhysicsCollisionValidator(currentEntry.ignoredLayers));
        // 프리뷰 및 배치 시작
        if (currentType == BlockType.Block)
        {
            mode = Mode.Block;
            placer.StartPlacement(currentEntry.prefab);
        }
        else if (currentType == BlockType.Path)
        {
            mode = Mode.WaitingForPathStart;
        }
        else
        {
            // other types (unit, etc.)
            mode = Mode.None;
        }
    }

    private void OnPlacePerformed(InputAction.CallbackContext ctx)
    {
        if (currentType == BlockType.None) return;
        if (!TryGetGridPosition(out var pos)) return;
        
        if (mode == Mode.Block)
        {
            placer.ConfirmPlacement(pos);
            ResetState();
        }
        else if (mode == Mode.WaitingForPathStart)
        {
            // require clicking on existing node
            if (registry.GetNodeAt(pos) != null)
            {
                pathStart = pos;
                mode = Mode.PlacingPath;
                placer.StartPlacement(currentEntry.prefab);
            }
        }
        else if (mode == Mode.PlacingPath)
        {
            var end = AlignOnAxis(pathStart, pos);
            InstallPath(pathStart, end);
            placer.CancelPlacement();
            ResetState();
        }
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        if (mode == Mode.None) return;
        placer.CancelPlacement();
        ResetState();
    }

    private void ResetState()
    {
        mode = Mode.None;
        currentType = BlockType.None;
        currentEntry = default;
        BlockSelection.Clear();
    }

    private bool TryGetGridPosition(out Vector3Int pos)
    {
        Vector2 mouse = InputManager.Instance.MousePosition;
        Ray ray = cam.ScreenPointToRay(mouse);
        LayerMask mask = groundMask;
        if (Physics.Raycast(ray, out var hit, 100f, mask))
        {
            Vector3 p = hit.point;
            pos = new Vector3Int(
                Mathf.FloorToInt(p.x),
                0,
                Mathf.FloorToInt(p.z)
            );
            return true;
        }
        pos = default;
        return false;
    }
    
    private Vector3Int AlignOnAxis(Vector3Int a, Vector3Int b)
    {
        var dx = Mathf.Abs(b.x - a.x);
        var dz = Mathf.Abs(b.z - a.z);
        return dx >= dz ? new Vector3Int(b.x, 0, a.z)
            : new Vector3Int(a.x, 0, b.z);
    }

    private void InstallPath(Vector3Int start, Vector3Int end)
    {
        var dir = (end - start);
        dir.x = dir.x == 0 ? 0 : dir.x / Mathf.Abs(dir.x);
        dir.z = dir.z == 0 ? 0 : dir.z / Mathf.Abs(dir.z);

        for (var pos = start + dir; ; pos += dir)
        {
            var tileGO = Instantiate(currentEntry.prefab, pos, Quaternion.identity);
            var tile   = tileGO.GetComponent<PathTile>();
            tile.Initialize(pos);
            registry.Register(tile);  // 점유 정보만 저장

            if (pos == end - dir) break;
        }

        // 2) 로직 연결 (블록 노드만)
        var fromNode = registry.GetNodeAt(start) as IConnectable;
        var toNode   = registry.GetNodeAt(end)   as IConnectable;
        if (fromNode != null && toNode != null)
        {
            // PathConnection 생성 시 ConnectNext/Prev까지 처리
            var connection = new PathConnection(fromNode, toNode);
            PathConnectionManager.Instance.RegisterConnection(connection);
            // (필요하다면) connections 리스트에 담아두고 관리
        }
    }
}
