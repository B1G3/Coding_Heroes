using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceRecoder : MonoBehaviour
{
    [Header("녹음 토글 액션")]
    [SerializeField] private InputActionReference recordAction;

    [Tooltip("샘플 레이트")] public int sampleRate = 16000;
    
    private AudioClip clip;
    private bool isRecording;

    public static event Action OnVoiceRecordStart;
    public static event Action<AudioClip> OnVoiceRecordComplete;

    public void Enter()
    {
        recordAction.action.Enable();
        recordAction.action.performed += OnRecordStarted;
        recordAction.action.canceled  += OnRecordStopped;
    }

    public void Exit()
    {
        recordAction.action.performed -= OnRecordStarted;
        recordAction.action.canceled  -= OnRecordStopped;
        recordAction.action.Disable();
    }
    
    private void OnRecordStarted(InputAction.CallbackContext ctx)
    {
        if (!isRecording)
            StartVoiceRecording();
    }

    private void OnRecordStopped(InputAction.CallbackContext ctx)
    {
        if (isRecording)
        {
            clip = StopAndRecognize();
            OnVoiceRecordComplete?.Invoke(clip);
        }
    }

    private void StartVoiceRecording()
    {
        if (Microphone.devices.Length == 0) return;
        clip = Microphone.Start(
            null, 
            false, 
            300, 
            sampleRate);
        isRecording = true;
        Debug.Log("녹음 시작");
        
        OnVoiceRecordStart?.Invoke();
    }

    private AudioClip StopAndRecognize()
    {
        if (!isRecording) return null;
    
        int lastSample = Microphone.GetPosition(null);
        Microphone.End(null);
        isRecording = false;
    
        // 실제 녹음된 길이만큼 AudioClip 트림
        if (lastSample > 0) {
            float[] samples = new float[lastSample * clip.channels];
            clip.GetData(samples, 0);
        
            AudioClip trimmedClip = AudioClip.Create("RecordedAudio", 
                lastSample, clip.channels, clip.frequency, false);
            trimmedClip.SetData(samples, 0);
        
            Debug.Log($"녹음 종료 - 실제 샘플: {lastSample}, 주파수: {clip.frequency}Hz");
            return trimmedClip;
        }
    
        Debug.Log("녹음 종료");
        return clip;
    }
}
