using UnityEngine;
using System.Collections.Generic;
using OpenAI;
using TMPro;
using DG.Tweening;

public class LLM : MonoBehaviour
{
    #region OpenAI
    public TextMeshProUGUI TextBox;
    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    public async void AskChatGPT(string text)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = text;
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

    #endregion
}
