using System.Threading.Tasks;
using UnityEngine;

public interface ILLMService
{
    public void Init(AdaptorData adaptorData);

    // ChatBot with selected LLM
    public Task<string> Chat(string inputText);
    public Task<string> Chat(string inputText, Texture2D inputImage);
}