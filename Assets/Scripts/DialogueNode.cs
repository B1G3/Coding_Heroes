using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    [TextArea(2, 5)]
    public string text;

    [Tooltip("재생할 음성 클립")]
    public AudioClip clip;

    [Tooltip("다이얼로그를 활성화할 조건 (Runtime에서 설정)")]
    public BoolEventReference condition;

    [HideInInspector]
    public bool shown = false;
}

[Serializable]
public class BoolEventReference
{
    public BoolEvent onCheck;

    /// <summary>
    /// 조건을 검사하여 결과 반환
    /// </summary>
    public bool IsTrue()
    {
        return onCheck?.Invoke() ?? false;
    }
}

[Serializable]
public class BoolEvent : UnityEngine.Events.UnityEvent<bool>
{
    /// <summary>
    /// 기본 생성자: 반환값 없음 → true 전달
    /// </summary>
    public bool Invoke()
    {
        // 이 이벤트는 리스너가 직접 상태를 전달해야 함
        // 예시: event.AddListener((state) => { /* ... */ });
        return true;
    }
}