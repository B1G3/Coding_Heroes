using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BlockConfig;

public class FunctionNode : IGridNode, IConnectable, ILogicalModule, IInput, IOutput, IGetData
{
    [Header("Function Settings")]
    [SerializeField] private int functionId = 1; // 🎯 함수 ID
    [SerializeField] private string functionName = "MyFunction";
    
    public FunctionStartNode FunctionStart { get; private set; }
    
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    [SerializeField] private LocalDirection dataInputDirection = LocalDirection.Left;
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }
    public IConnectable DataInput { get; set; }
    
    [SerializeField] private string next;
    [SerializeField] private string prev;
    [SerializeField] private string dataInput;
    
    // 함수 필드와의 연결
    
    
    
    private ITarget inputData;
    private List<Gnome> currentGnomes = new List<Gnome>();
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
        dataInputDirection = dataInputDirection.RotateY(steps);
        name = $"Function({functionName})";
        
        // 🎯 FunctionManager에 등록
        RegisterToManager();
        
        // 🎯 FunctionStart 찾기
        FindFunctionStart();
    }
    
    private void RegisterToManager()
    {
        if (FunctionManager.Instance != null)
        {
            FunctionManager.Instance.RegisterFunctionNode(this, functionId);
        }
    }
    
    private void FindFunctionStart()
    {
        if (FunctionManager.Instance != null)
        {
            FunctionStart = FunctionManager.Instance.GetFunctionStart(this);
        }
    }
    
    private void OnDestroy()
    {
        // 🎯 매니저에서 해제
        if (FunctionManager.Instance != null)
        {
            FunctionManager.Instance.UnregisterFunctionNode(this);
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
        Prev = prev;
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
    
    // 🎯 데이터 입력 연결 (좌측)
    public void ConnectDataInput(IConnectable dataInput)
    {
        DataInput = dataInput;
        this.dataInput = dataInput?.ToString();
    }
    
    public void DisconnectDataInput()
    {
        DataInput = null;
        dataInput = null;
    }
    
    // 🎯 IGetData 구현
    public void GetData(ITarget data)
    {
        inputData = data;
        Debug.Log($"FunctionNode: 데이터 받음 - {data}");
    }

    // 🎯 ILogicalModule 구현
    public async UniTaskVoid OnSignalEnter(List<IUnitState> command, List<Gnome> gnomes)
    {
        currentGnomes = gnomes;
        Debug.Log($"FunctionNode: {functionName} 호출 시작");
        // 🎯 FunctionStart 찾기
        FindFunctionStart();
        
        if (FunctionStart != null)
        {
            // 함수 필드로 신호 전송
            await FunctionStart.ExecuteFunction(command, gnomes, inputData, this);
        }
        else
        {
            Debug.LogError($"FunctionNode: {functionName}에 연결된 함수 필드가 없습니다!");
            await ContinueToNext(command, gnomes);
        }
    }
    
    // 함수 실행 완료 후 FunctionEndNode가 호출
    public async UniTaskVoid OnFunctionComplete(List<IUnitState> command, List<Gnome> gnomes)
    {
        Debug.Log($"FunctionNode: {functionName} 실행 완료 - 다음 노드로 진행");
        
        // 🎯 노움들을 FunctionNode 위치로 먼저 복귀시키기
        TeleportGnomesToFunction(gnomes);
        
        await ContinueToNext(command, gnomes);
    }

    // 🎯 노움들을 FunctionNode 위치로 순간이동 (다른 노드들처럼)
    private void TeleportGnomesToFunction(List<Gnome> gnomes)
    {
        Vector3 functionPosition = transform.position;
        
        foreach (var gnome in gnomes)
        {
            if (gnome != null)
            {
                // 순간이동으로 FunctionNode 위치로 복귀
                gnome.transform.position = functionPosition;
                Debug.Log($"노움 {gnome.name}을 FunctionNode 위치로 복귀: {functionPosition}");
            }
        }
        
        Debug.Log($"총 {gnomes.Count}마리 노움을 FunctionNode로 복귀 완료");
    }
    
    // 🎯 Input/Output Direction 구현
    public WorldDirection GetInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(inputDirection);
    }
    
    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }
    
    public WorldDirection GetDataInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(dataInputDirection);
    }
    
    private async UniTask ContinueToNext(List<IUnitState> command, List<Gnome> gnomes)
    {
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
}