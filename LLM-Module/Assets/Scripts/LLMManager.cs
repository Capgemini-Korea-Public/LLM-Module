using UnityEngine;
using System.Collections.Generic;
using OpenAI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class LLMManager : MonoBehaviour
{
    private void Start()
    {
        Ollama.InitChat();
    }

    #region OpenAI
    public TextMeshProUGUI TextBox;
    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();


    public async void AskChatGPT(string inputText)
    {
        ChatMessage newMessage = new ChatMessage();
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

    #region Sentis
    
    #endregion
}
