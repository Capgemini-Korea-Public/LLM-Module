// NativeAdapter wraps the internal LLM instance and implements the ILLMService interface.
// This adapter uses the native (internal) LLM execution method (e.g., gguf model) provided by LLM.cs.
using LLMUnity;
using System.Threading.Tasks;
using UnityEngine;
public class NativeAdapter : ILLMService
{
    private LLM _llm;

    // Constructor that initializes the adapter with an internal LLM instance.
    public NativeAdapter(LLM llm)
    {
        _llm = llm;
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

    public async Task<Ret> PostRequest<Res, Ret>(string json, string endpoint, ContentCallback<Res, Ret> getContent, Callback<Ret> callback = null)
    {
        Debug.Log("[LocalRun]-----------------------------------------------");
        // send a post request to the server and call the relevant callbacks to convert the received content and handle it
        // this function has streaming functionality i.e. handles the answer while it is being received
        while (!_llm.failed && !_llm.started) await Task.Yield();
        string callResult = null;
        switch (endpoint)
        {
            case "tokenize":
                callResult = await _llm.Tokenize(json);
                break;
            case "detokenize":
                callResult = await _llm.Detokenize(json);
                break;
            case "embeddings":
                callResult = await _llm.Embeddings(json);
                break;
            case "slots":
                callResult = await _llm.Slot(json);
                break;
            default:
                LLMUnitySetup.LogError($"Unknown endpoint {endpoint}");
                break;
        }

        Ret result = ConvertContent(callResult, getContent);
        callback?.Invoke(result);
        return result;
    }
}
