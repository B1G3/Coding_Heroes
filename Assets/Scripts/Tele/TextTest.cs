using TMPro;
using UnityEngine;

public class TextTest : MonoBehaviour
{
    [SerializeField] private bool isNPC;
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (isNPC)
        {
            VoiceInteractionManager.OnBotTextReceived += TextReceived;
        }
        else
        {
            VoiceInteractionManager.OnSttTextReceived += TextReceived;
        }
    }

    private void OnDisable()
    {
        if (isNPC)
        {
            VoiceInteractionManager.OnBotTextReceived -= TextReceived;
        }
        else
        {
            VoiceInteractionManager.OnSttTextReceived -= TextReceived;
        }
    }
    
    private void TextReceived(string input)
    {
        text.text = input;
    }
}
