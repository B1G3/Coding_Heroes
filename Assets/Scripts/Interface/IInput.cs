using UnityEngine;

public interface IInput
{
    BlockConfig.WorldDirection GetInputDirection(Transform transform);
    bool CanReceiveInput();
    void ReceiveInput(IOutput source);
    void DisconnectInput();
}