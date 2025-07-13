using System.Collections;
using TMPro;
using UnityEngine;

public class LexyDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float typingSpeed = 0.05f;

    private DialogueNode[] nodes;
    
    public void Initialize(DialogueNode[] nodes)
    {
        this.nodes = nodes;
        StartCoroutine(RunDialogue());
    }

    public void SetNewDialogue(DialogueNode[] nodes)
    {
        this.nodes = nodes;
        StartCoroutine(RunDialogue());
    }

    private IEnumerator RunDialogue()
    {
        foreach (var node in nodes)
        {
            if (node.shown) continue;

            // 조건 만족 후 대사 재생
            yield return StartCoroutine(PlayNode(node));
            node.shown = true;
        }
    }

    private IEnumerator PlayNode(DialogueNode node)
    {
        dialogueObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = "";

        // 오디오 재생
        audioSource.clip = node.clip;
        audioSource.Play();

        // 타자기 효과
        var sb = new System.Text.StringBuilder(node.text.Length);
        foreach (var c in node.text)
        {
            sb.Append(c);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(typingSpeed);
        }

        // 음성 끝날 때까지 대기
        yield return new WaitWhile(() => audioSource.isPlaying);

        dialogueText.gameObject.SetActive(false);
        dialogueObject.SetActive(false);
    }
}