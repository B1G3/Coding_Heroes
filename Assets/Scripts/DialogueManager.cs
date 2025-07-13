using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private LexyDialogue lexyDialogue;
    public static DialogueManager Instance { get; private set; }
    
    [SerializeField] private DialogueSetup dialogueSetup;
    [SerializeField] private DialogueNode[] dialogueNodes;
    [SerializeField] private DialogueNode[] dialogueAfterVoice;
    
    void Awake() => Instance = this;
    
    void OnEnable()
    {
        VoiceRecoder.OnVoiceRecordStart += VoiceRecordStart;
        VoiceRecoder.OnVoiceRecordStop += VoiceRecordEnd;
        LeverController.OnLeverMax += HandleLeverTriggered;
    }

    void OnDisable()
    {
        VoiceRecoder.OnVoiceRecordStart -= VoiceRecordStart;
        VoiceRecoder.OnVoiceRecordStop -= VoiceRecordEnd;
        LeverController.OnLeverMax -= HandleLeverTriggered;
    }

    public void SetLexyDialogue(LexyDialogue lexyDialogue)
    {
        this.lexyDialogue = lexyDialogue;
        this.lexyDialogue.Initialize(dialogueSetup.SetInitialDialogueNodes());
    }

    private void HandleLeverTriggered()
    {
        lexyDialogue.SetNewDialogue(dialogueNodes);
    }

    private void VoiceRecordStart()
    {
        var node = DialogueNode.Create("녹음 중...", null);
        lexyDialogue.SetNewDialogue(new[] { node, node });
    }
    
    private void VoiceRecordEnd()
    {
        var node = DialogueNode.Create("생성 중...", null);
        lexyDialogue.SetNewDialogue(new[] { node, node, node, dialogueAfterVoice[0] });
    }
    
    // 이런 식으로 하나 만들면 될듯
    // var node = DialogueNode.Create("안녕!", someClip);
    // lexyDialogue.SetNewDialogue(new[] { node });
}