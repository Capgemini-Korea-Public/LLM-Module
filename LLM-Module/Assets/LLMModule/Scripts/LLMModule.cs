using UnityEngine;
using System.Collections.Generic;
using OpenAI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using LLMUnity;
/// <summary>
/// Using LLMUnity, OpenAI, Ollama All in One Module
/// Using For ChatBot AI
/// </summary>

public class LLMModule : MonoBehaviour
{
    // RESTfulAPI, LocalAPI, NativeLibrary 선택
    [SerializeField] private EAPIType apiType;

    // Adaptor를 여기에 적용
    ILLMService llmService;

    // EModelType에 따라 다른 llmService를 끼워준다.
    private void Awake()
    {
        switch(apiType){
            case EAPIType.Native:
                llmService = new LocalLibraryAdaptor();
                break;
            case EAPIType.LocalhostAPI:
                llmService = new LocalhostAPIAdaptor();
                break;
            case EAPIType.RestfulAPI:
                llmService = new RESTfulAPIAdaptor();
                break;
        }
    }

    private void Start()
    {
        llmService.Init();
    }

    // FOR TEST
    public void Chat(TMP_InputField inputField)
    {
        ChatBot(inputField.text);
    }

    public async void ChatBot(string inputText)
    {
        Debug.Log($"{llmService.GetType().Name} start");
        await llmService.Chat(inputText);
    }

    /*
    #region OpenAI
    public TextMeshProUGUI TextBox;
    private OpenAIApi openAI = new OpenAIApi();
    private List<OpenAI.ChatMessage> messages = new List<OpenAI.ChatMessage>();

    public void Ask(TMP_InputField inputField)
    {
        AskChatGPT(inputField.text);
    }

    public async void AskChatGPT(string inputText)
    {
        OpenAI.ChatMessage newMessage = new OpenAI.ChatMessage();
        newMessage.Content = inputText;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
        }

    }
    #endregion
    */
    #region Ollama
    private Queue<string> buffer = new Queue<string>();
    private string model = "llama3.1";
    public async void AskViaOllama(string inputText)
    {
        Debug.Log("AskViaOllama Start");
        await Task.Run(async () =>
            await Ollama.ChatStream((string text) => buffer.Enqueue(text), model, inputText)
        );

        Debug.Log("AskViaOllama Done");
        string result = string.Empty;
        while (buffer.Count > 0)
            result += buffer.Dequeue();

        Debug.Log(result);
        //TextBox.DOText(result, 0.5f);
    }
    #endregion

    #region LLMUnity
    void HandleReply(string reply)
    {
        Debug.Log(reply);
        //TextBox.text = reply;
    }

    void ReplyCompleted()
    {
        Debug.Log("AI Reply Done");
    }

    public void Run(TMP_InputField input)
    {
        AskViaLLMUnity(input.text);
    }
    private async void AskViaLLMUnity(string inputText)
    {
        //string reply = await llmCharacter.Chat(inputText, HandleReply, ReplyCompleted);
    }
    #endregion
}

