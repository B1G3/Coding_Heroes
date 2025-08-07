using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VoiceInteractionManager : MonoBehaviour 
{
    [Header("API 설정")]
    [Tooltip("예: http://localhost:8000")]
    [SerializeField] string apiBaseUrl;

    [Header("Dependencies")]
    [SerializeField] VoiceRecoder recorder;
    
    [Header("Audio")]
    [SerializeField] AudioClip errorAudioClip;

    private IServerClient client;

    public static event Action<string> OnSttTextReceived;
    public static event Action<string> OnBotTextReceived;
    public static event Action<string, AudioClip> OnResponseReceived;

    void Awake() {
        if (string.IsNullOrEmpty(apiBaseUrl))
            Debug.LogError("API Base URL이 설정되지 않았습니다.");
        client = new ApiService(apiBaseUrl);
    }

    private void OnEnable()
    {
        VoiceRecoder.OnVoiceRecordComplete += RecordComplete;
    }
    
    private void OnDisable()
    {
        VoiceRecoder.OnVoiceRecordComplete -= RecordComplete;
    }

    private void RecordComplete(AudioClip clip)
    {
        ProcessClip(clip).Forget();
    }

    private async UniTask ProcessClip(AudioClip clip) 
    {
        // WAV 바이트 변환
        byte[] wavData = WavUtility.AudioClipToWav(clip);

        // 1) STT 호출
        string userText;
        try
        {
            userText = await client.SendSTT(wavData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"STT 호출 실패: {ex}");
            OnSttTextReceived?.Invoke("연결을 확인해주세요.");
            OnResponseReceived?.Invoke("네트워크 통신이 잘 안되는 것 같아", errorAudioClip);
            return;
        }

        OnSttTextReceived?.Invoke(userText);

        // 2) QA 챗봇 호출
        ChatbotResponse chatResp;
        try
        {
            chatResp = await client.SendChat(userText);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Chat 호출 실패: {ex}");
            return;
        }

        // 3) 정상 응답 처리
        if (chatResp != null)
        {
            byte[] audioBytes = Convert.FromBase64String(chatResp.audio);
            var ttsClip = WavUtility.ToAudioClip(audioBytes, "NPCVoice");
            OnResponseReceived?.Invoke(chatResp.answer, ttsClip);
        }
    }
}