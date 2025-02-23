using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LocalhostAPIAdaptor : ILLMService
{
    private Queue<string> buffer;
    private string model;

    public void Init(AdaptorData adaptorData)
    {
        LocalhostAdaptorData localhostAdaptorData = adaptorData as LocalhostAdaptorData;
        Ollama.InitChat();
        buffer = new Queue<string>();
        model = localhostAdaptorData.modelName;
    }


    public async Task<string> Chat(string inputText)
    {
        Debug.Log("Ollama Start");
        await Task.Run(async () =>
            await Ollama.ChatStream((string text) => buffer.Enqueue(text), model, inputText)
        );

        string res = string.Empty;
        while (buffer.Count > 0)
            res += buffer.Dequeue();

        Debug.Log(res);
        return res;
    }
}