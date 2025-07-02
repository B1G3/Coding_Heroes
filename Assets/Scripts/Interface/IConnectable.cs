
public interface IConnectable
{
    IConnectable Next { get; set; }
    IConnectable Prev { get; set; }

    void ConnectNext(IConnectable next);
    void ConnectPrev(IConnectable prev);
    void DisconnectNext();
    void DisconnectPrev();
}