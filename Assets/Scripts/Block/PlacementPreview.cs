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

        blockPreview = Instantiate(previewPrefab, transform);
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

    //이거 지금 이상함 개애바임
    private void UpdatePathPreview(Vector3 worldPos, Vector3Int gridPos)
    {
        var go = pathPool.Get();
        go.SetActive(true);
        go.transform.position = GridUtils.CellToWorld(gridPos);

        var rend = go.GetComponentInChildren<Renderer>();
        // 풀링된 오브젝트도 새 머티리얼로 인스턴스 생성
        rend.material = new Material(rend.sharedMaterial);
        var c = rend.material.color;
        c.a = 0.5f;
        rend.material.color = c;

        activePathPreviews.Add(go);
    }

    public void ClearPreview()
    {
        if (blockPreview != null)
            blockPreview.SetActive(false);

        foreach (var go in activePathPreviews)
            pathPool.Release(go);
        activePathPreviews.Clear();
    }
}