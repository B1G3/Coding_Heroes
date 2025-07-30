using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour {
    [SerializeField] int sampleRate = 16000;
    private AudioClip clip;
    private bool isRecording;

    public void StartRecording() {
        if (isRecording || Microphone.devices.Length == 0) return;
        clip = Microphone.Start(null, false, 300, sampleRate);
        isRecording = true;
        Debug.Log("녹음 시작");
    }

    public AudioClip StopRecording() {
        if (!isRecording) return null;
        Microphone.End(null);
        isRecording = false;
        Debug.Log("녹음 종료");
        return clip;
    }
}