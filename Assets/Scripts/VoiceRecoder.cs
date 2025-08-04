using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceRecoder : MonoBehaviour
{
    [Header("녹음 토글 액션")]
    [SerializeField] private InputActionReference recordAction;
    
    [Header("녹음 설정")] [Tooltip("녹음할 최대 길이 (초)")]
    public int maxRecordSeconds = 10;

    [Tooltip("샘플 레이트")] public int sampleRate = 44100;

    private AudioSource audioSource;
    private AudioClip recordingClip;
    private bool isRecording;

    public static event Action OnVoiceRecordStart;
    public static event Action OnVoiceRecordStop;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnEnable()
    {
        recordAction.action.Enable();
        recordAction.action.performed += OnRecordStarted;
        recordAction.action.canceled  += OnRecordStopped;
    }

    void OnDisable()
    {
        recordAction.action.performed -= OnRecordStarted;
        recordAction.action.canceled  -= OnRecordStopped;
        recordAction.action.Disable();
    }

    private void OnRecordStarted(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.isPlacing) return;
        if (!isRecording)
            StartVoiceRecording();
    }

    private void OnRecordStopped(InputAction.CallbackContext ctx)
    {
        if (isRecording)
            StopAndRecognize();
    }

    public void StartVoiceRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("녹음할 마이크가 없습니다.");
            return;
        }

        // 마이크 디바이스(기본)로 녹음 시작
        recordingClip = Microphone.Start(
            null, // 디바이스 이름 (null = 기본)
            false, // loop? (false = 한 번만)
            maxRecordSeconds, // 녹음 길이
            sampleRate // 샘플 레이트
        );
        isRecording = true;
        Debug.Log("녹음 시작...");
        
        OnVoiceRecordStart?.Invoke();
    }

    public void StopAndRecognize()
    {
        if (!isRecording) return;

        // 녹음 종료
        Microphone.End(null);
        isRecording = false;
        Debug.Log("녹음 종료!");

        // 👉 AudioSource에 연결해서 바로 재생해 볼 수 있습니다.
        audioSource.clip = recordingClip;
        audioSource.Play();
        
        OnVoiceRecordStop?.Invoke();

        // 👉 원하면 파일로 저장
        // SaveWavFile(recordingClip, "RecordedAudio.wav");
    }

    private void SaveWavFile(AudioClip clip, string filename)
    {
        // if (clip == null) return;
        // string path = Path.Combine(Application.persistentDataPath, filename);
        //
        // // SavWav 유틸리티가 프로젝트에 들어 있다면 이렇게 호출
        // bool ok = SavWav.Save(path, clip);
        // Debug.Log(ok
        //     ? $"WAV 파일 저장 완료: {path}"
        //     : "WAV 파일 저장 실패");
    }
}
