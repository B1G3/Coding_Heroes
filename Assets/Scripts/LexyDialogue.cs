using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LexyDialogue : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference leftGrabAction;
    [SerializeField] private InputActionReference rightGrabAction;
    
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Material faceMaterial;
    [SerializeField] private Sprite[] faceSprite;
    [SerializeField] private Sprite faceSpriteDefault;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private int maxCharsPerPage = 36;
    [SerializeField] private float pageDelay = 1.5f;

    private DialogueNode[] nodes;
    private bool skipRequested;

    private void OnEnable()
    {
        VoiceInteractionManager.OnResponseReceived += GetResponse;
        if (leftGrabAction != null)
        {
            leftGrabAction.action.performed += OnSkip; 
            leftGrabAction.action.Enable();
        }
        
        if (rightGrabAction != null)
        {
            rightGrabAction.action.performed += OnSkip; 
            rightGrabAction.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        VoiceInteractionManager.OnResponseReceived -= GetResponse;
        if (leftGrabAction != null)
        {
            leftGrabAction.action.performed -= OnSkip;
            leftGrabAction.action.Disable();
        }
        
        if (rightGrabAction != null)
        {
            rightGrabAction.action.performed -= OnSkip; 
            rightGrabAction.action.Disable();
        }
    }
    
    private void OnSkip(InputAction.CallbackContext ctx)
    {
        skipRequested = true;
    }
    
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

    private void GetResponse(string text, AudioClip clip)
    {
        var node = DialogueNode.Create(text, clip);
        SetNewDialogue(new[] { node });
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
        // 초기화
        skipRequested = false;
        dialogueObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        // 1) 오디오 재생
        audioSource.clip = node.clip;
        audioSource.Play();

        // 2) 문장→페이지 분할
        var pages    = BuildPages(SplitToSentences(node.text));

        // 3) 각 페이지 타입라이팅 & 페이지 대기
        foreach (var page in pages)
        {
            dialogueText.text = "";
            var sb = new StringBuilder();

            // 타입라이팅
            foreach (var c in page)
            {
                if (skipRequested)
                {
                    dialogueText.text = page;
                    break;
                }
                sb.Append(c);
                dialogueText.text = sb.ToString();
                yield return new WaitForSeconds(typingSpeed);
            }

            skipRequested = false;

            // 페이지 끝 대기 (직접 프레임 단위로 체크)
            float timer = 0f;
            while (timer < pageDelay && !skipRequested)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            skipRequested = false;
        }

        // 4) 마지막 페이지 대기 후, 음성 끝날 때까지 (스킵 가능)
        while (audioSource.isPlaying && !skipRequested)
            yield return null;

        skipRequested = false;

        // 5) UI 닫기
        dialogueText.gameObject.SetActive(false);
        dialogueObject.SetActive(false);
    }
    
    private List<string> SplitToSentences(string text)
    {
        // [마침표|물음표|느낌표] + 공백 기준으로 분리
        var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
        return new List<string>(sentences);
    }

    private  List<string> BuildPages(List<string> sentences)
    {
        var pages = new List<string>();
        var sb    = new StringBuilder();
    
        foreach (var s in sentences)
        {
            // 다음 문장을 붙여도 여유가 있으면
            if (sb.Length + s.Length <= maxCharsPerPage)
            {
                sb.Append(s).Append(' ');
            }
            else
            {
                // 페이지 완성
                pages.Add(sb.ToString().Trim());
                sb.Clear();
                sb.Append(s).Append(' ');
            }
        }
        if (sb.Length > 0)
            pages.Add(sb.ToString().Trim());
        return pages;
    }
    
}