using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VoiceInteractionManager : MonoBehaviour {
    public enum State { Idle, Recording, Uploading, Playing }
    public State CurrentState { get; private set; } = State.Idle;

    [Header("API 설정")]
    [Tooltip("예: http://localhost:8000")]
    [SerializeField] string apiBaseUrl;

    [Header("Dependencies")]
    [SerializeField] MicrophoneRecorder recorder;
    [SerializeField] AudioSource audioSource;

    private IServerClient client;

    public static event Action<string> OnSttTextReceived;
    public static event Action<string> OnBotTextReceived;
    public static event Action<AudioClip> OnAudioReceived;

    void Awake() {
        if (string.IsNullOrEmpty(apiBaseUrl))
            Debug.LogError("API Base URL이 설정되지 않았습니다.");
        client = new ApiService(apiBaseUrl);
    }

    public void OnRecordButtonPressed() {
        if (CurrentState == State.Idle) {
            recorder.StartRecording();
            CurrentState = State.Recording;
        } else if (CurrentState == State.Recording) {
            var clip = recorder.StopRecording();
            if (clip != null) ProcessClip(clip).Forget();
        }
    }

    private async UniTask ProcessClip(AudioClip clip) {
        CurrentState = State.Uploading;

        // WAV 바이트 변환
        byte[] wavData = WavUtility.FromAudioClip(clip);

        // 1) STT 호출
        string userText = await client.SendSTT(wavData);
        OnSttTextReceived?.Invoke(userText);

        // 2) QA 챗봇 호출
        var chatResp = await client.SendChat(userText);
        if (chatResp != null) {
            OnBotTextReceived?.Invoke(chatResp.answer);

            // 3) base64 음성 디코딩 및 재생
            byte[] audioBytes = Convert.FromBase64String(chatResp.audio);
            var ttsClip = WavUtility.ToAudioClip(audioBytes, "NPCVoice");
            audioSource.clip = ttsClip;
            CurrentState = State.Playing;
            audioSource.Play();
            OnAudioReceived?.Invoke(ttsClip);

            // 4) 재생 완료 대기
            await UniTask.WaitUntil(() => !audioSource.isPlaying);
        }

        CurrentState = State.Idle;
    }
}