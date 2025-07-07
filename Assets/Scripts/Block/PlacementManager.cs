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
    [SerializeField] private GameObject rotationPrefab;
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

        // 충돌 검사 설정 갱신
        placer.SetCollisionValidator(new PhysicsCollisionValidator(currentEntry.ignoredLayers));
        // 프리뷰 및 배치 시작
        if (currentType == BlockType.Block || currentType == BlockType.Unit)
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
        
        // Debug.Log($"[PlacementManager] Placing block at {pos}, mode: {mode}, type: {currentType}");
        
        if (mode == Mode.Block)
        {
            placer.ConfirmPlacement(pos);
            ResetState();
        }
        
        // 1) 시작 전(WaitingForPathStart)엔 기존 노드 클릭만 허용
        else if (mode == Mode.WaitingForPathStart)
        {
            // require clicking on existing node
            if (registry.GetNodeAt(pos) != null)
            {
                pathStart = pos;
                mode = Mode.PlacingPath;
                placer.StartPlacement(currentEntry.prefab);
            }

            return;
        }
        
        // 2) 직선 구간 배치 중
        if (mode == Mode.PlacingPath)
        {
            var clickedIsNode = registry.GetNodeAt(pos) != null;
            var aligned = AlignOnAxis(pathStart, pos);

            // 블록(노드)을 클릭했다면: 직선 설치 후 완료
            if (clickedIsNode)
            {
                InstallStraight(pathStart, aligned);
                ConnectNodes(pathStart, aligned);
                placer.CancelPlacement();
                ResetState();
            }
            else
            {
                // 땅을 클릭했다면: 직선 설치 → 회전 블록 설치 → 다음 세그먼트 준비
                InstallStraight(pathStart, aligned);
                PlaceRotationBlock(aligned);
                // 다음 직선은 이 회전 블록 위치부터
                pathStart = aligned;
                mode = Mode.PlacingPath;
                placer.StartPlacement(currentEntry.prefab);
            }
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
            pos = Vector3Int.RoundToInt(hit.point);
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

    /// <summary>
    /// start에서 end까지 한 축 직선 PathTile을 설치합니다.
    /// </summary>
    private void InstallStraight(Vector3Int start, Vector3Int end)
    {
        var dir = end - start;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.z = Mathf.Clamp(dir.z, -1, 1);
        for (var p = start + dir; ; p += dir)
        {
            var tileGO = Instantiate(currentEntry.prefab, p, Quaternion.identity);
            var tile   = tileGO.GetComponent<PathTile>();
            tile.Initialize(p);
            registry.Register(tile); 
            if (p == end - dir) break;
        }
    }

    /// <summary>
    /// 회전 블록 프리팹을 at 위치에 배치하고, IConnectable로 등록합니다.
    /// </summary>
    private void PlaceRotationBlock(Vector3Int at)
    {
        var go = Instantiate(rotationPrefab, at, Quaternion.identity);
        if (go.TryGetComponent<IGridNode>(out var gridNode) &&
            go.TryGetComponent<IConnectable>(out var conn))
        {
            gridNode.Initialize(at);
            registry.Register(gridNode);
            placer.CancelPlacement();
            // 앞 세그먼트의 끝 블록과 연결
            var from = registry.GetNodeAt(pathStart) as IConnectable;
            from?.ConnectNext(conn);
            conn?.ConnectPrev(from);
        }
    }

    /// <summary>
    /// start와 end 지점의 블록 노드를 IConnectable로 연결해 줍니다.
    /// </summary>
    private void ConnectNodes(Vector3Int start, Vector3Int end)
    {
        var from = registry.GetNodeAt(start) as IConnectable;
        var to   = registry.GetNodeAt(end)   as IConnectable;
        Debug.Log($"[PlacementManager] Connecting {start} to {end} / from: {from}, to: {to}");
        if (from != null && to != null)
        {
            PathConnectionManager.Instance.RegisterConnection(new PathConnection(from, to));
        }
    }

}
