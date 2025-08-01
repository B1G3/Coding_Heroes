using System.Collections.Generic;
using UnityEngine;

public class PlacementPreview : MonoBehaviour
{
    [Header("Block Holder")]
    [SerializeField] private BlockHolder leftBlockHolder;
    [SerializeField] private BlockHolder rightBlockHolder;
    
    [Header("Block pool")]
    [SerializeField] private GameObject previewPrefab;
    private GameObject blockPreview;
    private Renderer blockRend;
    
    [Header("Path pool")]
    [SerializeField] private PreviewPool pathPool;
    private readonly List<GameObject> activePathPreviews = new();
    
    // 머티리얼 캐싱으로 메모리 누수 방지
    private Material previewMaterial;
    
    private void Awake()
    {
        // 미리 프리뷰용 머티리얼 생성
        if (previewPrefab != null)
        {
            var renderer = previewPrefab.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                previewMaterial = new Material(renderer.sharedMaterial);
                var c = previewMaterial.color;
                c.a = 0.5f;
                previewMaterial.color = c;
            }
        }
    }
    
    private void OnEnable()
    {
        leftBlockHolder.OnPreviewBlock += UpdateBlockPreview; 
        rightBlockHolder.OnPreviewBlock += UpdateBlockPreview;
        leftBlockHolder.OnPreviewPath  += UpdatePathPreview;
        rightBlockHolder.OnPreviewPath += UpdatePathPreview;
        leftBlockHolder.OnDestroyPreview += ClearPreview;
        rightBlockHolder.OnDestroyPreview += ClearPreview;
    }
    
    private void OnDisable()
    {
        leftBlockHolder.OnPreviewBlock -= UpdateBlockPreview; 
        rightBlockHolder.OnPreviewBlock -= UpdateBlockPreview;
        leftBlockHolder.OnPreviewPath  -= UpdatePathPreview;
        rightBlockHolder.OnPreviewPath -= UpdatePathPreview;
        leftBlockHolder.OnDestroyPreview -= ClearPreview;
        rightBlockHolder.OnDestroyPreview -= ClearPreview;
    }

    public void CreatePreview()
    {
        if (blockPreview != null) return;

        blockPreview = Instantiate(previewPrefab, transform.position, GridUtils.GetOriginRotation());
        blockRend = blockPreview.GetComponentInChildren<Renderer>();

        // 원본 머티리얼을 건드리지 않도록 인스턴스 생성
        blockRend.material = new Material(blockRend.sharedMaterial);
        var c = blockRend.material.color;
        c.a = 0.5f;  // 반투명
        blockRend.material.color = c;
    }

    private void UpdateBlockPreview(Vector3 worldPos, bool isValid)
    {
        CreatePreview();
        blockPreview.SetActive(true);

        var cell = GridUtils.WorldToCell(worldPos);
        blockPreview.transform.position = GridUtils.CellToWorld(cell);

        blockRend.material.color = isValid
            ? new Color(0, 1, 0, 0.5f)   // 초록
            : new Color(1, 0, 0, 0.5f);  // 빨강
    }

    private void UpdatePathPreview(Vector3 startWorldPos, Vector3Int endGridPos)
    {
        // 기존 패스 프리뷰 정리
        ClearPathPreviews();
        
        // 시작점을 그리드 좌표로 변환
        var startGridPos = GridUtils.WorldToCell(startWorldPos);
        
        // 경로상의 모든 위치 계산 (실제 배치 로직과 동일)
        var pathPositions = CalculatePathPositions(startGridPos, endGridPos);
        
        // 경로가 유효한지 확인 (중간에 블록이 있는지 체크)
        bool isPathValid = IsPathValid(startGridPos, endGridPos, pathPositions);
        
        // 각 위치에 프리뷰 오브젝트 생성
        foreach (var gridPos in pathPositions)
        {
            var go = pathPool.Get();
            go.SetActive(true);
            go.transform.position = GridUtils.CellToWorld(gridPos);

            var rend = go.GetComponentInChildren<Renderer>();
            // 새로운 머티리얼 인스턴스 생성 (색상 변경을 위해)
            rend.material = new Material(previewMaterial);
            
            // 경로 유효성에 따라 색상 설정
            var color = isPathValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
            rend.material.color = color;

            activePathPreviews.Add(go);
        }
    }
    
    private List<Vector3Int> CalculatePathPositions(Vector3Int start, Vector3Int end)
    {
        var positions = new List<Vector3Int>();
        
        // 실제 PlaceFromToWorldPosition 로직과 동일하게 계산
        var dir = end - start;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.z = Mathf.Clamp(dir.z, -1, 1);
        
        // 시작점부터 끝점까지 순차적으로 추가
        for (var p = start + dir; ; p += dir)
        {
            positions.Add(p);
            if (p == end - dir) break;
        }
        
        return positions;
    }
    
    private bool IsPathValid(Vector3Int start, Vector3Int end, List<Vector3Int> pathPositions)
    {
        // 시작점에 블록이 있는지 확인
        if (!GridState.Instance.TryGetNode(start, out _))
            return false;
            
        // 끝점 확인 - 블록이 있거나 빈 공간이어야 함 (코너 설치 가능)
        // 이 부분은 게임 로직에 따라 조정 필요
        
        // 중간 경로에 블록이 있는지 확인
        foreach (var pos in pathPositions)
        {
            if (GridState.Instance.TryGetNode(pos, out _))
            {
                // 중간에 블록이 있으면 설치 불가
                return false;
            }
        }
        
        return true;
    }
    
    private void ClearPathPreviews()
    {
        foreach (var go in activePathPreviews)
            pathPool.Release(go);
        activePathPreviews.Clear();
    }

    public void ClearPreview()
    {
        if (blockPreview != null)
            blockPreview.SetActive(false);

        ClearPathPreviews();
    }
    
    private void OnDestroy()
    {
        // 메모리 정리
        if (previewMaterial != null)
            DestroyImmediate(previewMaterial);
    }
}