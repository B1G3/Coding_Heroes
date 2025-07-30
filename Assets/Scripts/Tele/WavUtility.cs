using System;
using UnityEngine;

public static class WavUtility {
    public static byte[] FromAudioClip(AudioClip clip) {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        byte[] bytes = new byte[samples.Length * 2];
        int offset = 0;
        foreach (var f in samples) {
            short s = (short)(Mathf.Clamp(f, -1f, 1f) * short.MaxValue);
            bytes[offset++] = (byte)(s & 0xFF);
            bytes[offset++] = (byte)((s >> 8) & 0xFF);
        }
        return bytes;
    }

    public static AudioClip ToAudioClip(byte[] pcmBytes, string name) {
        const int sampleRate = 16000;
        const int channels = 1;
        int sampleCount = pcmBytes.Length / 2;
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++) {
            short s = BitConverter.ToInt16(pcmBytes, i * 2);
            samples[i] = s / 32768f;
        }
        var clip = AudioClip.Create(name, sampleCount, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}