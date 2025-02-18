using UnityEngine;
using System.Collections.Generic;
using OpenAI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using LLMUnity;

public class LLMManager_TMP : MonoBehaviour
{
    LLMCharacter llmCharacter;
    LLM llm;

    private void Awake()
    {
        llmCharacter = GetComponent<LLMCharacter>();
        llm = GetComponent<LLM>();
    }

    private void Start()
    {
        Ollama.InitChat();
    }

    #region OpenAI
    public TextMeshProUGUI TextBox;
    private OpenAIApi openAI = new OpenAIApi();
    private List<OpenAI.ChatMessage> messages = new List<OpenAI.ChatMessage>();


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

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
            TextBox.DOText(chatResponse.Content, 0.5f);
        }
    }
    #endregion

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
        while(buffer.Count > 0)
            result += buffer.Dequeue();   
        
        Debug.Log(result);
        TextBox.DOText(result, 0.5f);
    }
    #endregion

    #region LLMUnity
    void HandleReply(string reply)
    {
        Debug.Log(reply);
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
        Debug.Log($"AskViaLLMUnity {llm.model} Start");
        string reply = await llmCharacter.Chat(inputText, HandleReply, ReplyCompleted);
    }
    #endregion
}
