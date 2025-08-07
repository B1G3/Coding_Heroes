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
        string userText = await client.SendSTT(wavData);
        Debug.Log(userText);
        OnSttTextReceived?.Invoke(userText);

        // 2) QA 챗봇 호출
        var chatResp = await client.SendChat(userText);
        Debug.Log(chatResp.answer);
        
        if (chatResp != null) {
            // 3) base64 음성 디코딩 및 재생
            byte[] audioBytes = Convert.FromBase64String(chatResp.audio);
            var ttsClip = WavUtility.ToAudioClip(audioBytes, "NPCVoice");
            OnResponseReceived?.Invoke(chatResp.answer, ttsClip);
        }
    }
}