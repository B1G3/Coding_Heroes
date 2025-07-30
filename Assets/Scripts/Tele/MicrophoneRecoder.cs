using System;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour {
    [SerializeField] int sampleRate = 16000;
    // private AudioSource audioSource;
    private AudioClip clip;
    private bool isRecording;

    // private void Awake()
    // {
    //     audioSource = GetComponent<AudioSource>();
    // }

    public void StartRecording() {
        if (isRecording || Microphone.devices.Length == 0) return;
        clip = Microphone.Start(null, false, 300, sampleRate);
        isRecording = true;
        Debug.Log("녹음 시작");
    }

    public AudioClip StopRecording() {
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