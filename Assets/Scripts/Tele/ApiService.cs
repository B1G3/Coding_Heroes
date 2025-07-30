using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class SttResponse {
    public string stt_result;
    public string status;
}

// 4) 챗봇 응답용 DTO
[Serializable]
public class ChatbotResponse {
    public string answer;
    public string audio;
    public string format;
    public string status;
}

public class ApiService : IServerClient
{
    private readonly string baseUrl;
    private readonly int timeoutSeconds;

    public ApiService(string baseUrl, int timeoutSeconds = 15)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
        this.timeoutSeconds = timeoutSeconds;
    }

    public async UniTask<string> SendSTT(byte[] wavData)
    {
        string url = $"{baseUrl}/ai_npc/stt";
        var form = new WWWForm();
        form.AddBinaryData("audio_file", wavData, "recording.wav", "audio/wav");

        using (var req = UnityWebRequest.Post(url, form))
        {
            req.timeout = timeoutSeconds;
            await req.SendWebRequest().ToUniTask();

            if (req.result == UnityWebRequest.Result.Success)
            {
                // Newtonsoft.Json으로 파싱
                var resp = JsonConvert.DeserializeObject<SttResponse>(req.downloadHandler.text);
                return resp.stt_result;
            }
            else
            {
                Debug.LogError($"STT Error: {req.error}");
                return string.Empty;
            }
        }
    }
    
    public async UniTask<ChatbotResponse> SendChat(string prompt) {
        string url = $"{baseUrl}/ai_npc/qa_chatbot";
        var payload = new { text = prompt };
        string jsonBody = JsonConvert.SerializeObject(payload);

        using (var req = new UnityWebRequest(url, "POST")) {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = timeoutSeconds;

            await req.SendWebRequest().ToUniTask();
            if (req.result == UnityWebRequest.Result.Success) {
                // Newtonsoft.Json으로 파싱
                var chatResp = JsonConvert.DeserializeObject<ChatbotResponse>(req.downloadHandler.text);
                return chatResp;
            } else {
                Debug.LogError($"Chat Error: {req.error}");
                return null;
            }
        }
    }
}