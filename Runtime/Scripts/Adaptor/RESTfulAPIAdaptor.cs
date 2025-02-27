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
        // Role, model 선택 
        restfulAdaptorData = adaptorData as RESTfulAdaptorData;

        // apiKey Setting
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string authFilePath = Path.Combine(homePath, ".openai", "auth.json");

        if (File.Exists(authFilePath))
        {
            // 파일 읽기
            string jsonContent = File.ReadAllText(authFilePath);
            // JSON 파싱 (예: Unity의 JsonUtility 또는 Newtonsoft.Json 사용)
            // API 키 추출 및 사용
            authData = JsonUtility.FromJson<AuthData>(jsonContent);

            // API 키 사용
            Debug.Log("API Key: " + authData.api_key);
        }
        else
        {
            Debug.LogError("auth.json 파일을 찾을 수 없습니다.");
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

        // Setting 값
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
        // apikey ignore에 추가
        // image Read/Write = true
        // Comprerssion = none 설정 필요
        string base64Image = Ollama.Texture2Base64(inputImage);

        // 메시지 구성
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

        // 전체 요청 본문
        var requestObj = new Dictionary<string, object>
    {
        { "model", "gpt-4o-mini" },
        { "messages", messages },
        { "max_tokens", 300 }
    };

        // JSON 직렬화
        string jsonRequest = JsonConvert.SerializeObject(requestObj);

        // 디버깅용 (요청 본문 시작 부분만 로깅)
        Debug.Log("Request JSON (first 500 chars): " + jsonRequest.Substring(0, Math.Min(500, jsonRequest.Length)) + "...");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);

        // API 요청
        using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {authData.api_key}");

            // 요청 전송
            await request.SendWebRequest();

            // 응답 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                Debug.Log("Response: " + responseJson);

                // JSON 응답 파싱
                var responseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                var choices = (responseObj["choices"] as Newtonsoft.Json.Linq.JArray);
                var message = (choices[0] as Newtonsoft.Json.Linq.JObject)["message"];
                string content = (string)((message as Newtonsoft.Json.Linq.JObject)["content"]);

                return content;
            }
            else
            {
                Debug.LogError($"API 요청 실패: {request.error}");
                Debug.LogError($"응답 코드: {request.responseCode}");
                Debug.LogError($"응답 내용: {request.downloadHandler?.text}");
                return $"오류 발생: {request.error}\n{request.downloadHandler?.text}";
            }
        }
    }
}