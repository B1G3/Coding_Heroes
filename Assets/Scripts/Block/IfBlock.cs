using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public class IfBlock : IGridNode
{
    [SerializeField] private IfNode ifNode;
    [SerializeField] private DataCollectorNode dataCollectorNode;
    
    private Vector3Int gridPosition;
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        gridPosition = gridPos;
        
        // IfBlock 자체 위치 설정
        transform.position = GridUtils.CellToWorld(gridPos);
        
        // Origin 기준 회전 적용 (ARGridPlacer와 동일한 방식)
        var origin = GridUtils.GetOrigin();
        if (origin != null)
        {
            transform.rotation = origin.rotation * Quaternion.Euler(0, rotationY, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        
        // 서브 노드들 초기화 및 등록
        InitializeSubNodes(rotationY);
        
        // 서브 노드들을 BlockManager에 등록
        RegisterSubNodes();
        
        // 서브 노드들 연결 (내부 연결만)
        dataCollectorNode.ConnectNext(ifNode);
        
        name = "IfBlock";
    }
    
    private void InitializeSubNodes(float rotationY)
    {
        // 서브노드들의 현재 월드 위치를 기준으로 그리드 위치 계산
        if (dataCollectorNode != null)
        {
            Vector3Int dataGridPos = GridUtils.WorldToCell(dataCollectorNode.transform.position);
            dataCollectorNode.Initialize(dataGridPos, rotationY);
            
            // GridState에 등록
            GridState.Instance.AddOrUpdateNode(dataGridPos, dataCollectorNode);
        }
        
        if (ifNode != null)
        {
            Vector3Int ifGridPos = GridUtils.WorldToCell(ifNode.transform.position);
            ifNode.Initialize(ifGridPos, rotationY);
            
            // GridState에 등록
            GridState.Instance.AddOrUpdateNode(ifGridPos, ifNode);
        }
    }
    
    private void RegisterSubNodes()
    {
        if (dataCollectorNode != null)
        {
            BlockManager.Instance.RegisterBlock(dataCollectorNode.gameObject);
        }
        
        if (ifNode != null)
        {
            BlockManager.Instance.RegisterBlock(ifNode.gameObject);
        }
    }
    
    public bool IsMultiGrid()
    {
        return true;
    }
    
    public List<Vector3Int> GetOccupiedGridPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        if (dataCollectorNode != null)
        {
            positions.Add(GridUtils.WorldToCell(dataCollectorNode.transform.position));
        }
        
        if (ifNode != null)
        {
            positions.Add(GridUtils.WorldToCell(ifNode.transform.position));
        }
        
        return positions;
    }
    
    public IfNode GetIfNode() => ifNode;
    public DataCollectorNode GetDataCollectorNode() => dataCollectorNode;
    
    private void OnDestroy()
    {
        if (BlockManager.Instance != null)
        {
            if (dataCollectorNode != null)
                BlockManager.Instance.UnregisterBlock(dataCollectorNode.gameObject);
            
            if (ifNode != null)
                BlockManager.Instance.UnregisterBlock(ifNode.gameObject);
        }
    }
}