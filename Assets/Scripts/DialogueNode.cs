using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    [TextArea(2, 5)]
    public string text;

    [Tooltip("재생할 음성 클립")]
    public AudioClip clip;

    [HideInInspector]
    public bool shown = false;
    
    /// <summary>
    /// 런타임에 인스턴스 생성과 동시에 필드 초기화
    /// </summary>
    public static DialogueNode Create(string text, AudioClip clip)
    {
        var instance = CreateInstance<DialogueNode>();
        instance.text  = text;
        instance.clip  = clip;
        instance.shown = false;
        return instance;
    }
}