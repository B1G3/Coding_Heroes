using UnityEngine;
using System.Collections.Generic;

public class FunctionArea : IGridNode
{
    [Header("Function Area Settings")]
    [SerializeField] private int functionId = 1;
    [SerializeField] private string functionName = "MyFunction";
    
    [Header("Area Size")]
    [SerializeField] private Vector2Int areaSize = new Vector2Int(3, 3); // 기본 3x3
    
    [Header("Function Nodes")]
    [SerializeField] private FunctionStartNode functionStart;
    [SerializeField] private FunctionEndNode functionEnd;
    
    private Vector3Int gridPosition;
    
    public FunctionStartNode FunctionStart => functionStart;
    public FunctionEndNode FunctionEnd => functionEnd;
    public Vector2Int AreaSize => areaSize;
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        gridPosition = gridPos;
        
        // 🎯 FunctionArea 자체 위치 설정 (IfBlock과 동일한 방식)
        transform.position = GridUtils.CellToWorld(gridPos);
        
        // Origin 기준 회전 적용
        var origin = GridUtils.GetOrigin();
        if (origin != null)
        {
            transform.rotation = origin.rotation * Quaternion.Euler(0, rotationY, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        
        // 🎯 서브 노드들 초기화 및 등록
        InitializeSubNodes(rotationY);
        
        // 🎯 서브 노드들을 BlockManager에 등록
        RegisterSubNodes();
        
        // 🎯 FunctionManager에 등록
        RegisterToManager();
        
        // 🎯 Start와 End 서로 연결
        ConnectStartAndEnd();
        
        name = $"FunctionArea({functionName})";
    }
    
    private void InitializeSubNodes(float rotationY)
    {
        // 🎯 FunctionStart는 실제 위치 기준
        if (functionStart != null)
        {
            Vector3Int startGridPos = GridUtils.WorldToCell(functionStart.transform.position);
            functionStart.Initialize(startGridPos, rotationY);
        
            // GridState에 등록
            GridState.Instance.AddOrUpdateNode(startGridPos, functionStart);
            Debug.Log($"✅ FunctionStart 등록: 그리드 {startGridPos}, 월드 {functionStart.transform.position}");
        }
    
        // 🎯 FunctionEnd도 실제 위치 기준으로!
        if (functionEnd != null)
        {
            Vector3Int endGridPos = GridUtils.WorldToCell(functionEnd.transform.position);
            functionEnd.Initialize(endGridPos, rotationY);
        
            // GridState에 등록
            GridState.Instance.AddOrUpdateNode(endGridPos, functionEnd);
            Debug.Log($"✅ FunctionEnd 등록: 그리드 {endGridPos}, 월드 {functionEnd.transform.position}");
        }
    }

    
    private void RegisterSubNodes()
    {
        if (functionStart != null)
        {
            BlockManager.Instance.RegisterBlock(functionStart.gameObject);
        }
        
        if (functionEnd != null)
        {
            BlockManager.Instance.RegisterBlock(functionEnd.gameObject);
        }
    }
    
    private void RegisterToManager()
    {
        if (FunctionManager.Instance != null && functionStart != null)
        {
            FunctionManager.Instance.RegisterFunction(functionId, functionName, functionStart);
        }
    }
    
    private void ConnectStartAndEnd()
    {
        if (functionStart != null && functionEnd != null)
        {
            functionStart.ConnectedFunctionEnd = functionEnd;
            functionEnd.ConnectedFunctionStart = functionStart;
            
            // 같은 Function ID 설정
            functionStart.SetFunctionId(functionId, functionName);
        }
    }
    
    // 🎯 멀티 그리드 지원
    public bool IsMultiGrid()
    {
        return true;
    }
    
    public List<Vector3Int> GetOccupiedGridPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        if (functionStart != null)
        {
            positions.Add(GridUtils.WorldToCell(functionStart.transform.position));
        }
        
        if (functionEnd != null)
        {
            positions.Add(GridUtils.WorldToCell(functionEnd.transform.position));
        }
        
        // 🎯 추후 확장: 전체 area 범위의 모든 그리드 위치 추가 가능
        // for (int x = 0; x < areaSize.x; x++)
        // {
        //     for (int z = 0; z < areaSize.y; z++)
        //     {
        //         positions.Add(gridPosition + new Vector3Int(x, 0, z));
        //     }
        // }
        
        return positions;
    }
    
    private void OnDestroy()
    {
        // BlockManager에서 해제
        if (BlockManager.Instance != null)
        {
            if (functionStart != null)
                BlockManager.Instance.UnregisterBlock(functionStart.gameObject);
            
            if (functionEnd != null)
                BlockManager.Instance.UnregisterBlock(functionEnd.gameObject);
        }
        
        // FunctionManager에서 해제
        if (FunctionManager.Instance != null)
        {
            FunctionManager.Instance.UnregisterFunction(functionId);
        }
    }
}