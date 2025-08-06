using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private LexyDialogue lexyDialogue;
    public static DialogueManager Instance { get; private set; }
    
    [SerializeField] private DialogueSetup dialogueSetup;
    [SerializeField] private DialogueNode[] dialogueNodes;
    
    void Awake() => Instance = this;
    
    void OnEnable()
    {
        LeverController.OnLeverMax += HandleLeverTriggered;
    }

    void OnDisable()
    {
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
}