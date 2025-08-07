using System.Collections.Generic;
using UnityEngine;

public class FunctionManager : MonoBehaviour
{
    public static FunctionManager Instance { get; private set; }
    
    [Header("Function Mapping")]
    // 🎯 FunctionNode → 함수 ID 매핑
    private Dictionary<FunctionNode, int> functionNodeToId = new Dictionary<FunctionNode, int>();
    
    // 🎯 함수 ID → FunctionStartNode 매핑  
    private Dictionary<int, FunctionStartNode> idToFunctionStart = new Dictionary<int, FunctionStartNode>();
    
    // 🎯 함수 ID → 함수 이름 (디버깅/표시용)
    private Dictionary<int, string> idToFunctionName = new Dictionary<int, string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 🎯 함수 등록
    public void RegisterFunction(int functionId, string functionName, FunctionStartNode functionStart)
    {
        idToFunctionStart[functionId] = functionStart;
        idToFunctionName[functionId] = functionName;
        Debug.Log($"FunctionManager: 함수 등록 - ID:{functionId}, Name:{functionName}");
    }
    
    // 🎯 함수 노드 등록
    public void RegisterFunctionNode(FunctionNode functionNode, int functionId)
    {
        functionNodeToId[functionNode] = functionId;
        Debug.Log($"FunctionManager: 함수 노드 등록 - {functionNode.name} → ID:{functionId}");
    }
    
    // 🎯 함수 노드가 자신의 FunctionStart 찾기
    public FunctionStartNode GetFunctionStart(FunctionNode functionNode)
    {
        if (functionNodeToId.TryGetValue(functionNode, out int functionId))
        {
            if (idToFunctionStart.TryGetValue(functionId, out FunctionStartNode functionStart))
            {
                Debug.Log($"FunctionManager: 함수 찾음 - {functionNode.name} → {idToFunctionName[functionId]}");
                return functionStart;
            }
        }
        
        Debug.LogError($"FunctionManager: {functionNode.name}에 해당하는 함수를 찾을 수 없습니다!");
        return null;
    }
    
    // 🎯 함수 해제
    public void UnregisterFunction(int functionId)
    {
        idToFunctionStart.Remove(functionId);
        idToFunctionName.Remove(functionId);
    }
    
    public void UnregisterFunctionNode(FunctionNode functionNode)
    {
        functionNodeToId.Remove(functionNode);
    }
}