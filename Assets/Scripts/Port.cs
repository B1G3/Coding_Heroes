using UnityEngine;
using static BlockConfig;

[System.Serializable]
public class Port
{
    public LocalDirection localDirection;
    public PortType portType;
    public bool isConnected;
    
    public Port(LocalDirection dir, PortType type)
    {
        localDirection = dir;
        portType = type;
        isConnected = false;
    }
    
    // 오브젝트의 rotation을 고려해서 월드 방향 계산
    public WorldDirection GetWorldDirection(Transform transform)
    {
        return DirectionUtils.LocalToWorldDirection(localDirection, transform);
    }
}