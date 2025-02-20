using LLMUnity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class LocalhostAPIAdaptor : ILLMService
{
    public Action OnStreamFinished;

    private const string SERVER = "http://localhost:11434/";

    private class Endpoints
    {
        public string GENERATE = "api/generate";
        public string CHAT = "api/chat";
        public string LIST = "api/tags";
        public string EMBEDDINGS = "api/embed";
    }

    Ret ConvertContent<Res, Ret>(string response, ContentCallback<Res, Ret> getContent = null)
    {
        // template function to convert the json received and get the content
        if (response == null) return default;
        response = response.Trim();
        if (response.StartsWith("data: "))
        {
            string responseArray = "";
            foreach (string responsePart in response.Replace("\n\n", "").Split("data: "))
            {
                if (responsePart == "") continue;
                if (responseArray != "") responseArray += ",\n";
                responseArray += responsePart;
            }
            response = $"{{\"data\": [{responseArray}]}}";
        }
        return getContent(JsonUtility.FromJson<Res>(response));
    }

    public async Task<Ret> PostRequest<Res, Ret>(string json, string endpoint, ContentCallback<Res, Ret> getContent, Callback<Ret> callback)
    {
        if (endpoint != "completion") return default;
        Debug.Log("[Ollama Run]-------------------------------");
        HttpWebRequest httpWebRequest;
        Endpoints ep = new Endpoints();
        endpoint = ep.CHAT;
        try
        {
            httpWebRequest = (HttpWebRequest)WebRequest.Create($"{SERVER}{endpoint}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync());
            streamWriter.Write(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n\t{e.StackTrace}");
            return default;
        }

        var httpResponse = await httpWebRequest.GetResponseAsync();
        using var streamReader = new StreamReader(httpResponse.GetResponseStream());

        string callResult = await streamReader.ReadToEndAsync();
        Ret result = ConvertContent(callResult, getContent);
        callback?.Invoke(result);
        return result;
    }
}