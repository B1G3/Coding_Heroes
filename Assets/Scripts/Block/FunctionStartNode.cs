using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FunctionStartNode : IGridNode, IConnectable, ILogicalModule, IOutput
{
    [Header("Function Settings")]
    [SerializeField] private int functionId = 1; // 🎯 함수 ID
    [SerializeField] private string functionName = "MyFunction";
    
    public FunctionNode Caller { get; private set; }
    public FunctionEndNode ConnectedFunctionEnd { get; set; }
    
    [SerializeField] private BlockConfig.LocalDirection outputDirection = BlockConfig.LocalDirection.Forward;
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; } // 함수에서는 사용 안함
    
    [SerializeField] private string next;
    [SerializeField] private string prev;
    
    private ITarget _parameter;
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        outputDirection = outputDirection.RotateY(steps);
        name = $"FunctionStart({functionName})";
        
        // 🎯 FunctionManager에 등록
        RegisterToManager();
        
        // 함수 끝 노드와의 연결 설정
        if (ConnectedFunctionEnd != null)
        {
            ConnectedFunctionEnd.ConnectedFunctionStart = this;
        }
    }
    
    private void RegisterToManager()
    {
        if (FunctionManager.Instance != null)
        {
            FunctionManager.Instance.RegisterFunction(functionId, functionName, this);
        }
    }
    
    private void OnDestroy()
    {
        // 🎯 매니저에서 해제
        if (FunctionManager.Instance != null)
        {
            FunctionManager.Instance.UnregisterFunction(functionId);
        }
    }
    
    // 🎯 IConnectable 구현
    public void ConnectNext(IConnectable next)
    {
        Next = next;
        this.next = next?.ToString();
    }

    public void ConnectPrev(IConnectable prev)
    {
        Prev = prev; // 함수에서는 실제로 사용 안함
        this.prev = prev?.ToString();
    }

    public void DisconnectNext()
    {
        Next = null;
        next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
        prev = null;
    }
    
    // 🎯 IOutput 구현
    public BlockConfig.WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }
    
    // 🎯 ILogicalModule 구현 (일반적인 신호 전파용)
    public async UniTaskVoid OnSignalEnter(List<IUnitState> command, List<Gnome> gnomes)
    {
        Debug.Log("FunctionStartNode: 일반 신호 받음 (함수 호출 아님)");
        
        if (Next != null)
        {
            Vector3 nextPosition = (Next as MonoBehaviour).transform.position;
            foreach (var gnome in gnomes)
            {
                gnome.SetTarget(nextPosition);
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            }
            await WaitForGnomesToReachNext(gnomes);
        }
        
        (Next as ILogicalModule)?.OnSignalEnter(command, gnomes).Forget();
    }
    
    // 🎯 함수 실행용 (FunctionNode에서 호출)
    public async UniTask ExecuteFunction(List<IUnitState> command, List<Gnome> gnomes, ITarget parameter, FunctionNode caller)
    {
        Caller = caller;
        _parameter = parameter;
        
        Debug.Log($"FunctionStartNode: 함수 실행 시작 (호출자: {caller.name})");
        
        TeleportGnomesToStart(gnomes);
        
        if (Next != null)
        {
            // 매개변수를 다음 노드들에게 전파
            PropagateParameter(Next, parameter);
            
            Vector3 nextPosition = (Next as MonoBehaviour).transform.position;
            foreach (var gnome in gnomes)
            {
                gnome.SetTarget(nextPosition);
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            }
            await WaitForGnomesToReachNext(gnomes);
        }
        
        // 함수 구현부로 신호 전파
        (Next as ILogicalModule)?.OnSignalEnter(command, gnomes).Forget();
    }
    
    private void TeleportGnomesToStart(List<Gnome> gnomes)
    {
        Vector3 startPosition = transform.position;
    
        foreach (var gnome in gnomes)
        {
            if (gnome != null)
            {
                // 순간이동으로 위치 설정
                gnome.transform.position = startPosition;
            
                Debug.Log($"노움 {gnome.name}을 FunctionStart({functionName}) 위치로 순간이동: {startPosition}");
            }
        }
    
        Debug.Log($"총 {gnomes.Count}마리 노움을 FunctionStart로 순간이동 완료");
    }

    
    private void PropagateParameter(IConnectable node, ITarget parameter)
    {
        if (node is IGetData dataReceiver)
        {
            dataReceiver.GetData(parameter);
        }
        
        if (node is IConnectable connectable && connectable.Next != null)
        {
            PropagateParameter(connectable.Next, parameter);
        }
    }
    
    private async UniTask WaitForGnomesToReachNext(List<Gnome> gnomes)
    {
        while (true)
        {
            bool allReached = true;
            foreach (var gnome in gnomes)
            {
                if (gnome != null && gnome.IsMoving)
                {
                    allReached = false;
                    break;
                }
            }
            
            if (allReached) break;
            await UniTask.Yield();
        }
    }

    public void SetFunctionId(int i, string s)
    {
        functionId = i;
        functionName = s;
    }
}