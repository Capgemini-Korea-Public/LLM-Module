using OpenAI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RESTfulAPIAdaptor : ILLMService
{
    private OpenAIApi openAI = new OpenAIApi();
    private List<OpenAI.ChatMessage> messages = new List<OpenAI.ChatMessage>();
    RESTfulAdaptorData restfulAdaptorData;
    public virtual void Init(AdaptorData adaptorData)
    {
        // Role, model º±≈√ 
        restfulAdaptorData = adaptorData as RESTfulAdaptorData;
    }

    public async Task<string> Chat(string inputText)
    {
        Debug.Log("Start Chat with GPT");
        OpenAI.ChatMessage newMessage = new OpenAI.ChatMessage();
        newMessage.Content = inputText;

        // Setting
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;

        // Setting ∞™
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);
        string res = string.Empty;
        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
            res = chatResponse.Content;
        }

        Debug.Log(res);
        return res;
    }
}