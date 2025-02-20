using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LocalhostAPIAdaptor : ILLMService
{
    private Queue<string> buffer;
    private string model;

    public void Init()
    {
        Ollama.InitChat();
        buffer = new Queue<string>();
        model = "llama3.1";
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