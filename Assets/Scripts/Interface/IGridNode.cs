using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public abstract class IGridNode : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    
    // 연결된 노드들 (월드 방향별로)
    protected Dictionary<WorldDirection, IGridNode> connections = new Dictionary<WorldDirection, IGridNode>();

    public virtual void Initialize(Vector3Int gridPos)
    {
        GridPosition = gridPos;
    }
    
    // Input 인터페이스 체크
    public bool HasInput()
    {
        return this is IInput;
    }
    
    // Output 인터페이스 체크  
    public bool HasOutput()
    {
        return this is IOutput;
    }
    
    
    // 특정 방향으로 연결 가능한지 확인
    public bool CanConnectTo(IGridNode other, WorldDirection direction)
    {
        // 내가 Output을 가지고 있고, 상대방이 Input을 가지고 있는지 확인
        if (this is IOutput myOutput && other is IInput otherInput)
        {
            var myOutputDir = myOutput.GetOutputDirection(transform);
            var otherInputDir = otherInput.GetInputDirection(other.transform);
            var oppositeDir = DirectionUtils.GetOppositeWorldDirection(direction);
            
            return myOutputDir == direction && 
                   otherInputDir == oppositeDir &&
                   myOutput.CanProvideOutput() && 
                   otherInput.CanReceiveInput();
        }
        
        return false;
    }
    
    // 노드 연결
    public bool ConnectTo(IGridNode other, WorldDirection direction)
    {
        if (!CanConnectTo(other, direction)) return false;
        
        var oppositeDir = DirectionUtils.GetOppositeWorldDirection(direction);
        
        // 연결 설정
        connections[direction] = other;
        other.connections[oppositeDir] = this;
        
        // 인터페이스를 통한 연결 처리
        if (this is IOutput myOutput && other is IInput otherInput)
        {
            myOutput.ConnectOutput(otherInput);
            otherInput.ReceiveInput(myOutput);
        }
        
        return true;
    }
    
    // 연결 해제
    public void DisconnectFrom(WorldDirection direction)
    {
        if (connections.TryGetValue(direction, out var other))
        {
            var oppositeDir = DirectionUtils.GetOppositeWorldDirection(direction);
            
            // 인터페이스를 통한 연결 해제
            if (this is IOutput myOutput)
            {
                myOutput.DisconnectOutput();
            }
            if (other is IInput otherInput)
            {
                otherInput.DisconnectInput();
            }
            
            // 연결 제거
            connections.Remove(direction);
            other.connections.Remove(oppositeDir);
        }
    }
    
    // 연결된 노드 가져오기
    public IGridNode GetConnectedNode(WorldDirection direction)
    {
        connections.TryGetValue(direction, out var node);
        return node;
    }
    
    // 모든 연결 가져오기
    public Dictionary<WorldDirection, IGridNode> GetAllConnections()
    {
        return new Dictionary<WorldDirection, IGridNode>(connections);
    }
}