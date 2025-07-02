using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PathConnection 정보를 보관하고 관리하는 싱글턴 매니저 클래스입니다.
/// </summary>
public class PathConnectionManager : MonoBehaviour
{
    public static PathConnectionManager Instance { get; private set; }

    [SerializeField]
    private List<PathConnection> _connections = new List<PathConnection>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 새로운 PathConnection을 등록합니다.
    /// </summary>
    public void RegisterConnection(PathConnection connection)
    {
        if (!_connections.Contains(connection))
            _connections.Add(connection);
    }

    /// <summary>
    /// 지정된 블록 노드에서 시작하는 모든 연결을 반환합니다.
    /// </summary>
    public IEnumerable<PathConnection> GetConnectionsFrom(IConnectable from)
    {
        foreach (var conn in _connections)
        {
            if (conn.From == from)
                yield return conn;
        }
    }

    /// <summary>
    /// 지정된 블록 노드로 연결되는 모든 연결을 반환합니다.
    /// </summary>
    public IEnumerable<PathConnection> GetConnectionsTo(IConnectable to)
    {
        foreach (var conn in _connections)
        {
            if (conn.To == to)
                yield return conn;
        }
    }

    /// <summary>
    /// 모든 연결 정보를 초기화합니다.
    /// </summary>
    public void ClearAllConnections()
    {
        _connections.Clear();
    }
}
