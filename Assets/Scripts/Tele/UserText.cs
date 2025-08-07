using System.Collections;
using TMPro;
using UnityEngine;

public class UserText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;
    
    private void OnEnable()
    {
        VoiceInteractionManager.OnSttTextReceived += SetText;
    }
    
    private void OnDisable()
    {
        VoiceInteractionManager.OnSttTextReceived -= SetText;
    }
    
    private void SetText(string newText)
    {
        // 이전 타이핑이 남아있으면 멈추기
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        // 새 코루틴 시작
        typingCoroutine = StartCoroutine(TypeText(newText));
    }

    private IEnumerator TypeText(string content)
    {
        text.text = "";

        foreach (char c in content)
        {
            text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null; // 완료 표시
    }
}
