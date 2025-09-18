using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
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
    
    private bool isBoss = false;

    private void OnEnable()
    {
        StageManager.OnStageChanged += StageSettingChanged;
    }
    
    private void OnDisable()
    {
        StageManager.OnStageChanged -= StageSettingChanged;
    }

    private void StageSettingChanged(StageConfig stage)
    {
        isBoss = stage.BossStage;
    }

    public ApiService(string baseUrl, int timeoutSeconds = 15)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
        this.timeoutSeconds = timeoutSeconds;
    }

    public async UniTask<string> SendSTT(byte[] wavFile)
    {
        // 2) multipart/form-data 폼 생성
        string url = $"{baseUrl}/ai_npc/stt";
        var form = new WWWForm();
        form.AddBinaryData(
            fieldName: "audio_file",
            contents: wavFile,
            fileName: "recording.wav",
            mimeType: "audio/wav"
        );

        // 3) 요청 전송
        using (var req = UnityWebRequest.Post(url, form))
        {
            req.timeout = timeoutSeconds;
            await req.SendWebRequest().ToUniTask();

            if (req.result == UnityWebRequest.Result.Success)
            {
                // 4) JSON 파싱
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
        var stageInfo = isBoss ? "boss" : "learn";
        var payload = new
        {
            text = prompt,
            stage = stageInfo,
        };
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