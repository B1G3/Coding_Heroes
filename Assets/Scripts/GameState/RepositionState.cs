using System;

public class RepositionState : IGameState
{
    public event Action Entered;
    public event Action Exited;

    public bool CanPlaceBlock => false;
    public bool CanRotateBlock => false;
    public bool CanReposition => true;

    public void Enter()
    {
        Entered?.Invoke();
    }

    public void Exit()
    {
        Exited?.Invoke();
    }
}