using Cysharp.Threading.Tasks;

public interface IServerClient {
    UniTask<string> SendSTT(byte[] wavData);
    UniTask<ChatbotResponse> SendChat(string prompt);
}