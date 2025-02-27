using Newtonsoft.Json;
using OpenAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class RESTfulAPIAdaptor : ILLMService
{
    [Serializable]
    private class AuthData
    {
        public string api_key;
        public string organization;
    }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    RESTfulAdaptorData restfulAdaptorData;

    string homePath;
    string authFilePath;
    AuthData authData;

    public virtual void Init(AdaptorData adaptorData)
    {
        // Role, model ���� 
        restfulAdaptorData = adaptorData as RESTfulAdaptorData;

        // apiKey Setting
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string authFilePath = Path.Combine(homePath, ".openai", "auth.json");

        if (File.Exists(authFilePath))
        {
            // ���� �б�
            string jsonContent = File.ReadAllText(authFilePath);
            // JSON �Ľ� (��: Unity�� JsonUtility �Ǵ� Newtonsoft.Json ���)
            // API Ű ���� �� ���
            authData = JsonUtility.FromJson<AuthData>(jsonContent);

            // API Ű ���
            Debug.Log("API Key: " + authData.api_key);
        }
        else
        {
            Debug.LogError("auth.json ������ ã�� �� �����ϴ�.");
        }
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

        // Setting ��
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

    public async Task<string> Chat(string inputText, Texture2D inputImage)
    {
        // apikey ignore�� �߰�
        // image Read/Write = true
        // Comprerssion = none ���� �ʿ�
        string base64Image = Ollama.Texture2Base64(inputImage);

        // �޽��� ����
        var messages = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            { "role", "user" },
            { "content", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "type", "text" }, { "text", inputText } },
                    new Dictionary<string, object>
                    {
                        { "type", "image_url" },
                        { "image_url", new Dictionary<string, string> { { "url", $"data:image/jpeg;base64,{base64Image}" } } }
                    }
                }
            }
        }
    };

        // ��ü ��û ����
        var requestObj = new Dictionary<string, object>
    {
        { "model", "gpt-4o-mini" },
        { "messages", messages },
        { "max_tokens", 300 }
    };

        // JSON ����ȭ
        string jsonRequest = JsonConvert.SerializeObject(requestObj);

        // ������ (��û ���� ���� �κи� �α�)
        Debug.Log("Request JSON (first 500 chars): " + jsonRequest.Substring(0, Math.Min(500, jsonRequest.Length)) + "...");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);

        // API ��û
        using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {authData.api_key}");

            // ��û ����
            await request.SendWebRequest();

            // ���� Ȯ��
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                Debug.Log("Response: " + responseJson);

                // JSON ���� �Ľ�
                var responseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                var choices = (responseObj["choices"] as Newtonsoft.Json.Linq.JArray);
                var message = (choices[0] as Newtonsoft.Json.Linq.JObject)["message"];
                string content = (string)((message as Newtonsoft.Json.Linq.JObject)["content"]);

                return content;
            }
            else
            {
                Debug.LogError($"API ��û ����: {request.error}");
                Debug.LogError($"���� �ڵ�: {request.responseCode}");
                Debug.LogError($"���� ����: {request.downloadHandler?.text}");
                return $"���� �߻�: {request.error}\n{request.downloadHandler?.text}";
            }
        }
    }
}