using TMPro;
using UnityEngine;

public class ChatBot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textBox;

    LLMModule llmModule;

    private void Awake()
    {
        llmModule = FindAnyObjectByType<LLMModule>();
    }
    public async void RunLLM(TMP_InputField textinput)
    {
        string res = await llmModule.Chat(textinput.text);
        textBox.text = res; 
    }
}
