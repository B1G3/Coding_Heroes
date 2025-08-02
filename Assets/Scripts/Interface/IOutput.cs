using UnityEngine;

public interface IOutput  
{
    BlockConfig.WorldDirection GetOutputDirection(Transform transform);
    bool CanProvideOutput();
    void ConnectOutput(IInput target);
    void DisconnectOutput();
}