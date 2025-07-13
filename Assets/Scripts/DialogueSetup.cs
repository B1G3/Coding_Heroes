using UnityEngine;

public class DialogueSetup : MonoBehaviour
{
    [SerializeField] private DialogueNode[] dialogueNodes;

    public DialogueNode[] SetInitialDialogueNodes()
    {
        return dialogueNodes;
    }
}