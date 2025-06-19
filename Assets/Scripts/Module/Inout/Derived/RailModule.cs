using UnityEngine;

public class RailModule : InoutModule
{
    private IInput connectedInput;

    public void Tick(Vector3Int myPos)
    {
        if (!TrySend(out int amount, GetOutputDirection())) return;

        var targetBox = FlowManager.Instance.FindBoxInDirection(myPos, GetOutputDirection());
        if (targetBox is IInput input && input.CanReceive(GetOutputDirection()))
        {
            input.TryReceive(amount, GetOutputDirection());
            Debug.Log($"[Rail] Sent {amount} to {targetBox} at {targetBox.GridPosition}");
        }
    }
}