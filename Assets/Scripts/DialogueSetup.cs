using UnityEngine;

public class DialogueSetup : MonoBehaviour
{
    [SerializeField] private DialogueNode[] dialogueNodes;
    [SerializeField] private DialogueConditionProvider provider;
    
    void Start()
    {
        // 예: 첫 번째 노드에 집 완성 조건 바인딩
        dialogueNodes[0].condition.onCheck.AddListener(
            _ => provider.isTrue()
        );
        
        // 예: 첫 번째 노드에 집 완성 조건 바인딩
        dialogueNodes[1].condition.onCheck.AddListener(
            _ => provider.isTrue()
        );

        // 두 번째 노드에 레버 당김 조건 바인딩
        dialogueNodes[2].condition.onCheck.AddListener(
            _ => provider.IsLeverTriggered()
        );
    }

    public DialogueNode[] SetDialogueNodes()
    {
        return dialogueNodes;
    }
}