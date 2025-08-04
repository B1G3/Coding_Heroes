using System;

public class RecordState : IGameState
{
    public event Action Entered;
    public event Action Exited;
    
    public bool CanPlaceBlock => false;
    public bool CanRotateBlock => false;
    public bool CanReposition => false;

    public void Enter()
    {
        Entered?.Invoke();
    }

    public void Exit()
    {
        Exited?.Invoke();
    }
}
