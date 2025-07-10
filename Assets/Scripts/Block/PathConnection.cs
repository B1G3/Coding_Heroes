using System;

[Serializable]
public class PathConnection
{
    public IConnectable From { get; }
    public IConnectable To   { get; }

    public PathConnection(IConnectable from, IConnectable to)
    {
        From = from; 
        To   = to;
        // 구독 방식으로도 가능하고, 직접 호출 방식도 가능
        From.ConnectNext(To);
        To.ConnectPrev(From);
        PathConnectionManager.Instance.RegisterConnection(this);
    }

    public void Trigger()
    {
        // 예: From이 작동했을 때
        // 1) Path 타일들의 애니메이션을 돌리고
        // 2) 마지막에 To.OnSignalEnter() 호출
        // To.OnSignalEnter();
    }
}