using UnityEngine;

public class DialogueConditionProvider : MonoBehaviour
{
    // HomeSpawner.OnHomeSpawned 이벤트 핸들러에서 세팅될 플래그
    private bool _homeSpawned = false;
    private bool _leverTriggered = false;

    void OnEnable()
    {
        HomeSpawner.OnHomeSpawned += HandleHomeSpawned;
        LeverController.OnLeverMax += HandleLeverTriggered;
    }

    void OnDisable()
    {
        HomeSpawner.OnHomeSpawned -= HandleHomeSpawned;
        LeverController.OnLeverMax -= HandleLeverTriggered;
    }

    private void HandleHomeSpawned(GameObject home)
    {
        _homeSpawned = true;
    }

    private void HandleLeverTriggered()
    {
        _leverTriggered = true;
    }

    /// <summary>
    /// DialogueNode 의 홈 조건으로 바인딩할 메서드
    /// </summary>
    public bool IsHouseSpawned()
    {
        return _homeSpawned;
    }
    
    /// <summary>
    /// DialogueNode 의 레버 조건으로 바인딩할 메서드 
    /// </summary>
    public bool IsLeverTriggered()
    {
        return _leverTriggered;
    }

    public bool isTrue()
    {
        return true;
    }
}