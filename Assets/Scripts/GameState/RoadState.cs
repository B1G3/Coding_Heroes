using System;

public class RoadState : IGameState
{
    public event Action Entered;
    public event Action Exited;
    
    public bool CanPlaceBlock => false;
    public bool CanRotateBlock => true;
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
