using TMPro;
using UnityEngine;

public class ChatBot : MonoBehaviour
{
    public Texture2D inputImage;
    [SerializeField] TextMeshProUGUI textBox;

    LLMModule llmModule;
    

    private void Awake()
    {
        llmModule = FindAnyObjectByType<LLMModule>();
    }
    public async void RunLLM(TMP_InputField textinput)
    {
        string res = await llmModule.Chat(textinput.text, inputImage);
        textBox.text = res; 
    }

    public async void RunLLMwithImage(TMP_InputField textinput, Texture2D inputImage)
    {
        string res = await llmModule.Chat(textinput.text, inputImage);
        textBox.text = res;
    }
}
