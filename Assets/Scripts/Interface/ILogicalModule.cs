using UnityEngine;

/// 신호가 들어왔을 때 호출되는 모듈용 인터페이스
public interface ILogicalModule
{
    /// 논리 신호(Flow)가 이 노드에 닿았을 때
    void OnSignalEnter(string signal = "");
}