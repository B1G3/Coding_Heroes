using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UserText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float indicatorSpeed = 0.5f; 

    private Coroutine typingCoroutine;
    private Coroutine indicatorCoroutine;
    
    private void OnEnable()
    {
        VoiceRecoder.OnVoiceRecordStart += HandleRecordStart;
        VoiceInteractionManager.OnSttTextReceived += SetText;
    }
    
    private void OnDisable()
    {
        VoiceRecoder.OnVoiceRecordStart -= HandleRecordStart;
        VoiceInteractionManager.OnSttTextReceived -= SetText;
    }
    
    private void HandleRecordStart()
    {
        // 기존 타이핑 코루틴이 있으면 멈추기
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // 녹음 인디케이터 코루틴 시작
        if (indicatorCoroutine != null)
            StopCoroutine(indicatorCoroutine);
        indicatorCoroutine = StartCoroutine(ShowRecordingIndicator());
    }
    
    private IEnumerator ShowRecordingIndicator()
    {
        const string baseMsg = "음성입력중";
        int dotCount = 0;
        var sb = new StringBuilder(baseMsg.Length + 4);
        while (true)
        {
            sb.Clear();
            sb.Append(baseMsg);
            for (int i = 0; i < dotCount; i++)
                sb.Append('.');
            text.text = sb.ToString();

            dotCount = (dotCount + 1) % 5;
            yield return new WaitForSeconds(indicatorSpeed);
        }
    }
    
    private void SetText(string newText)
    {
        // 이전 타이핑이 남아있으면 멈추기
        if (indicatorCoroutine != null)
        {
            StopCoroutine(indicatorCoroutine);
            indicatorCoroutine = null;
        }
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        // 새 코루틴 시작
        typingCoroutine = StartCoroutine(TypeText(newText));
    }

    private IEnumerator TypeText(string content)
    {
        var sb = new StringBuilder(content.Length);
        text.text = "";

        foreach (char c in content)
        {
            sb.Append(c);
            text.text = sb.ToString();
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }
}
