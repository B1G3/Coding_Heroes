using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public abstract class IGridNode : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    
    [Header("Port Configuration")]
    [SerializeField] protected List<Port> ports = new List<Port>();
    
    // 연결된 노드들 (월드 방향별로)
    protected Dictionary<WorldDirection, IGridNode> connections = new Dictionary<WorldDirection, IGridNode>();

    public virtual void Initialize(Vector3Int gridPos)
    {
        GridPosition = gridPos;
        InitializePorts();
    }
    
    // 각 블록 타입별로 오버라이드해서 포트 설정
    protected abstract void InitializePorts();
    
    // 로컬 방향의 포트 가져오기
    public Port GetLocalPort(LocalDirection localDirection)
    {
        return ports.Find(p => p.localDirection == localDirection);
    }
    
    // 월드 방향의 포트 가져오기
    public Port GetWorldPort(WorldDirection worldDirection)
    {
        var localDir = DirectionUtils.WorldToLocalDirection(worldDirection, transform);
        return GetLocalPort(localDir);
    }
    
    // 특정 월드 방향에 포트가 있는지 확인
    public bool HasWorldPort(WorldDirection worldDirection, PortType portType = PortType.Both)
    {
        var port = GetWorldPort(worldDirection);
        if (port == null) return false;
        
        return portType == PortType.Both || 
               port.portType == PortType.Both || 
               port.portType == portType;
    }
    
    // 연결 가능한지 확인
    public bool CanConnectTo(IGridNode other, WorldDirection worldDirection)
    {
        var myPort = GetWorldPort(worldDirection);
        if (myPort == null || myPort.isConnected) return false;
        
        var oppositeDir = DirectionUtils.GetOppositeWorldDirection(worldDirection);
        var otherPort = other.GetWorldPort(oppositeDir);
        if (otherPort == null || otherPort.isConnected) return false;
        
        // 포트 타입 호환성 확인
        return (myPort.portType == PortType.Output || myPort.portType == PortType.Both) &&
               (otherPort.portType == PortType.Input || otherPort.portType == PortType.Both);
    }
    
    // 노드 연결
    public bool ConnectTo(IGridNode other, WorldDirection worldDirection)
    {
        if (!CanConnectTo(other, worldDirection)) return false;
        
        var myPort = GetWorldPort(worldDirection);
        var oppositeDir = DirectionUtils.GetOppositeWorldDirection(worldDirection);
        var otherPort = other.GetWorldPort(oppositeDir);
        
        // 연결 설정
        connections[worldDirection] = other;
        other.connections[oppositeDir] = this;
        
        myPort.isConnected = true;
        otherPort.isConnected = true;
        
        return true;
    }
    
    // 연결 해제
    public void DisconnectFrom(WorldDirection worldDirection)
    {
        if (connections.TryGetValue(worldDirection, out var other))
        {
            var oppositeDir = DirectionUtils.GetOppositeWorldDirection(worldDirection);
            
            // 포트 연결 상태 해제
            GetWorldPort(worldDirection).isConnected = false;
            other.GetWorldPort(oppositeDir).isConnected = false;
            
            // 연결 제거
            connections.Remove(worldDirection);
            other.connections.Remove(oppositeDir);
        }
    }
    
    // 연결된 노드 가져오기
    public IGridNode GetConnectedNode(WorldDirection worldDirection)
    {
        connections.TryGetValue(worldDirection, out var node);
        return node;
    }
    
    // 모든 연결 가져오기
    public Dictionary<WorldDirection, IGridNode> GetAllConnections()
    {
        return new Dictionary<WorldDirection, IGridNode>(connections);
    }
}