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
    [SerializeField] private BlockBindings blockBindings;
    
    private BlockType currentType = BlockType.None;
    private PrefabEntry currentEntry;
    private Vector3Int gridPos;

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
            placer.UpdatePlacement(gridPos);
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
        placer.StartPlacement(currentEntry.prefab);
    }

    private void OnPlacePerformed(InputAction.CallbackContext ctx)
    {
        if (currentType == BlockType.None)
            return;

        placer.ConfirmPlacement(gridPos);
        ResetState();
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        if (currentType == BlockType.None)
            return;

        placer.CancelPlacement();
        ResetState();
    }

    private void ResetState()
    {
        currentType = BlockType.None;
        currentEntry = default;
    }

    private bool TryGetGridPosition(out Vector3Int pos)
    {
        Vector2 mouse = InputManager.Instance.MousePosition;
        Ray ray = cam.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out var hit, 100f, groundMask))
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
    
}
