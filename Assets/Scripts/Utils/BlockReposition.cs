using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockReposition : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference grabAction;    
    [SerializeField] private InputActionReference rotationAction;
    
    [Header("Hand Settings")]
    [SerializeField] private Transform holdPoint;
    
    [Header("LayerMasks")]
    [Tooltip("땅에만 설치 가능")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("길은 블럭부터 설치")]
    [SerializeField] private LayerMask blockLayer;
    
    [Header("Placement")]
    [SerializeField] private float placementCheckDistance = 0.2f;
    
    private int interactMask;
    private RaycastHit hit;
    private bool OnGround;
    private bool OnBlock;
    
    private GameObject selectedBlock;
    private IGridNode selectedGridNode;
    private Vector3Int originalGridPosition;
    private bool isRepositioning;
    
    private float currentRotationY = 0f;
    
    // 연결 해제 시 저장할 정보들
    private List<GameObject> disconnectedPaths = new List<GameObject>();
    private IConnectable originalNext;
    private IConnectable originalPrev;
    
    private void Awake()
    {
        interactMask = groundLayer.value | blockLayer.value;
    }
    
    public void Enter()
    {
        grabAction.action.Enable();
        rotationAction?.action.Enable();
    }
    
    public void Exit()
    {
        grabAction.action.Disable();
        rotationAction?.action.Disable();
        
        // 재배치 중이던 블럭이 있다면 원래 위치로 복원
        if (isRepositioning && selectedBlock != null)
        {
            RestoreBlockToOriginalPosition();
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.CanReposition()) return;
        
        ConfigPosition();
        
        if (rotationAction.action.WasPressedThisFrame() && isRepositioning)
        {
            RotateSelectedBlock();
        }
        
        // 블럭 선택
        if (!isRepositioning && grabAction.action.WasPressedThisFrame())
        {
            TrySelectBlock();
        }
        // 블럭 배치
        else if (isRepositioning && grabAction.action.WasReleasedThisFrame())
        {
            TryRepositionBlock();
        }
    }
    
    private void ConfigPosition()
    {
        if (Physics.Raycast(holdPoint.position, Vector3.down,
                out hit, placementCheckDistance, interactMask))
        {
            int mask = 1 << hit.collider.gameObject.layer;
            OnGround = (mask & groundLayer.value) != 0;
            OnBlock  = (mask & blockLayer.value)  != 0;
        }
        else
        {
            OnGround = OnBlock = false;
        }
    }
    
    private void TrySelectBlock()
    {
        if (!OnBlock) return;
        
        GameObject blockObject = BlockManager.Instance.GetBlockFromCollider(hit.collider);
        if (blockObject == null) return;
        
        selectedBlock = blockObject;
        selectedGridNode = blockObject.GetComponent<IGridNode>();
        
        if (selectedGridNode == null) return;
        
        originalGridPosition = selectedGridNode.GridPosition;
        
        // 블럭과 연결된 모든 관계를 끊기
        DisconnectBlock();
        
        isRepositioning = true;
        
        // 시각적 피드백 (블럭을 손에 따라 이동시키거나 하이라이트 등)
        // TODO: 블럭을 holdPoint 위치로 이동시키는 로직 추가
    }
    
    private void DisconnectBlock()
    {
        if (selectedGridNode is IConnectable connectable)
        {
            // 기존 연결 정보 저장
            originalNext = connectable.Next;
            originalPrev = connectable.Prev;
            
            // 이전 블럭과의 연결 해제
            if (originalPrev != null)
            {
                originalPrev.DisconnectNext();
                connectable.DisconnectPrev();
            }
            
            // 다음 블럭과의 연결 해제
            if (originalNext != null)
            {
                originalNext.DisconnectPrev();
                connectable.DisconnectNext();
            }
        }
        
        // 그리드에서 블럭 제거
        GridState.Instance.RemoveNode(originalGridPosition);
        
        // BlockManager에서 등록 해제
        BlockManager.Instance.UnregisterBlock(selectedBlock);
        
        // 연결되어 있던 패스들 제거
        RemoveConnectedPaths();
    }
    
    private void RemoveConnectedPaths()
    {
        disconnectedPaths.Clear();
        
        // 원래 위치에서 시작해서 연결된 패스들을 찾아 제거
        // 이 부분은 패스 검색 로직에 따라 구현이 달라질 수 있음
        Vector3Int[] directions = { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };
        
        foreach (var dir in directions)
        {
            Vector3Int checkPos = originalGridPosition + dir;
            
            if (GridState.Instance.TryGetNode(checkPos, out IGridNode node))
            {
                GameObject nodeObject = node.gameObject;
                
                // 패스인지 확인 (패스 식별 로직 필요)
                if (IsPath(nodeObject))
                {
                    RemovePathSequence(checkPos, dir);
                }
            }
        }
    }
    
    private bool IsPath(GameObject obj)
    {
        // 패스 식별 로직 (태그, 컴포넌트, 이름 등으로 구분)
        // 예시: return obj.CompareTag("Path");
        return obj.name.Contains("Path") || obj.name.Contains("Corner");
    }
    
    private void RemovePathSequence(Vector3Int startPos, Vector3Int direction)
    {
        Vector3Int currentPos = startPos;
        
        while (GridState.Instance.TryGetNode(currentPos, out IGridNode node))
        {
            GameObject pathObject = node.gameObject;
            
            if (!IsPath(pathObject)) break;
            
            // 패스 저장 (복원을 위해)
            disconnectedPaths.Add(pathObject);
            
            // 그리드에서 제거
            GridState.Instance.RemoveNode(currentPos);
            
            // BlockManager에서 해제
            BlockManager.Instance.UnregisterBlock(pathObject);
            
            // 오브젝트 비활성화 또는 제거
            pathObject.SetActive(false);
            
            currentPos += direction;
        }
    }
    
    private void TryRepositionBlock()
    {
        if (!OnGround) return;
        
        Vector3Int newGridPosition = GridUtils.WorldToCell(hit.point);
        
        // 이미 다른 블럭이 있는 위치인지 확인
        if (GridState.Instance.TryGetNode(newGridPosition, out _))
        {
            // 겹치는 위치에는 배치 불가
            return;
        }
        
        // 새 위치에 블럭 배치
        RepositionBlock(newGridPosition);
    }
    
    private void RepositionBlock(Vector3Int newGridPosition)
    {
        // 월드 좌표로 변환
        Vector3 worldPosition = GridUtils.CellToWorld(newGridPosition);
        worldPosition.y = hit.point.y;
        
        // 블럭 이동
        selectedBlock.transform.position = worldPosition;
        selectedBlock.transform.rotation = GridUtils.GetOriginRotation() * Quaternion.Euler(0f, currentRotationY, 0f);
        
        // 그리드 상태 업데이트
        selectedGridNode.Initialize(newGridPosition, currentRotationY);
        GridState.Instance.AddOrUpdateNode(newGridPosition, selectedGridNode);
        
        // BlockManager에 다시 등록
        BlockManager.Instance.RegisterBlock(selectedBlock);
        
        // 재배치 완료
        isRepositioning = false;
        selectedBlock = null;
        selectedGridNode = null;
        currentRotationY = 0f;
        
        // 제거된 패스들 정리
        CleanupDisconnectedPaths();
    }
    
    private void RestoreBlockToOriginalPosition()
    {
        if (selectedBlock == null || selectedGridNode == null) return;
        
        // 원래 위치로 복원
        Vector3 originalWorldPosition = GridUtils.CellToWorld(originalGridPosition);
        selectedBlock.transform.position = originalWorldPosition;
        
        // 그리드 상태 복원
        selectedGridNode.Initialize(originalGridPosition, 0f);
        GridState.Instance.AddOrUpdateNode(originalGridPosition, selectedGridNode);
        
        // BlockManager에 등록
        BlockManager.Instance.RegisterBlock(selectedBlock);
        
        // 연결 복원
        if (selectedGridNode is IConnectable connectable)
        {
            if (originalPrev != null)
            {
                connectable.ConnectPrev(originalPrev);
                originalPrev.ConnectNext(connectable);
            }
            
            if (originalNext != null)
            {
                connectable.ConnectNext(originalNext);
                originalNext.ConnectPrev(connectable);
            }
        }
        
        // 패스들 복원
        RestoreDisconnectedPaths();
        
        isRepositioning = false;
        selectedBlock = null;
        selectedGridNode = null;
    }
    
    private void CleanupDisconnectedPaths()
    {
        foreach (var path in disconnectedPaths)
        {
            if (path != null)
            {
                Destroy(path);
            }
        }
        disconnectedPaths.Clear();
    }
    
    private void RestoreDisconnectedPaths()
    {
        foreach (var path in disconnectedPaths)
        {
            if (path != null)
            {
                path.SetActive(true);
                var gridNode = path.GetComponent<IGridNode>();
                if (gridNode != null)
                {
                    GridState.Instance.AddOrUpdateNode(gridNode.GridPosition, gridNode);
                    BlockManager.Instance.RegisterBlock(path);
                }
            }
        }
        disconnectedPaths.Clear();
    }
    
    private void RotateSelectedBlock()
    {
        currentRotationY += 90f;
        if (currentRotationY >= 360f)
            currentRotationY = 0f;
            
        if (selectedBlock != null)
        {
            selectedBlock.transform.rotation = GridUtils.GetOriginRotation() * Quaternion.Euler(0f, currentRotationY, 0f);
        }
    }
}