using System;
using UnityEngine;

public static class WavUtility {
    public static byte[] AudioClipToWav(AudioClip clip)
    {
        if (clip == null) return null;
    
        Debug.Log($"AudioClip 정보 - 샘플: {clip.samples}, 채널: {clip.channels}, 주파수: {clip.frequency}Hz");
    
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
    
        byte[] pcmData = new byte[samples.Length * 2];
        int offset = 0;
        foreach (var f in samples) {
            short s = (short)(Mathf.Clamp(f, -1f, 1f) * short.MaxValue);
            pcmData[offset++] = (byte)(s & 0xFF);
            pcmData[offset++] = (byte)((s >> 8) & 0xFF);
        }
    
        // AudioClip의 실제 주파수와 채널 사용
        return GetWavFile(pcmData, clip.frequency, clip.channels);
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
    
    public static byte[] GetWavFile(byte[] pcmData, int sampleRate = 16000, int channels = 1)
    {
        int byteRate = sampleRate * channels * 2;
        int blockAlign = channels * 2;
        int dataSize = pcmData.Length;
        int fileSize = 44 + dataSize;

        using (var mem = new System.IO.MemoryStream())
        using (var writer = new System.IO.BinaryWriter(mem))
        {
            // RIFF chunk descriptor
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); // UTF8 -> ASCII
            writer.Write(fileSize - 8);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt sub-chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)blockAlign);
            writer.Write((short)16);

            // data sub-chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);
            writer.Write(pcmData);

            return mem.ToArray();
        }
    }
}