
public class Block : IGridNode, IConnectable
{
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
    }

    public void ConnectPrev(IConnectable prev)
    {
        Prev = prev;
    }

    public void DisconnectNext()
    {
        Next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
    }
}
