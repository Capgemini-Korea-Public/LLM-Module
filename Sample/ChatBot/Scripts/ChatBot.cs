using TMPro;
using UnityEngine;

public class ChatBot : MonoBehaviour
{
    LLMModule llmModule;

    private void Awake()
    {
        llmModule = FindAnyObjectByType<LLMModule>();
    }
    public void RunLLM(TMP_InputField textinput)
    {
        llmModule.Chat(textinput.text);
    }
}
