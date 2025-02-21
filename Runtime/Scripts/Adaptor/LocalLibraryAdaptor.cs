using LLMUnity;
using System.Threading.Tasks;
using UnityEngine;
public class LocalLibraryAdaptor : ILLMService
{
    LLM llm;
    LLMCharacter llmCharacter;
    LocalLibraryAdaptorData localLibraryAdaptorData;
    public void Init(AdaptorData adaptorData)
    {
        localLibraryAdaptorData = adaptorData as LocalLibraryAdaptorData;
        this.llm = localLibraryAdaptorData.llm;
        this.llmCharacter = localLibraryAdaptorData.llmCharacter;
    }

    public async Task<string> Chat(string inputText)
    {
        string res = await llmCharacter.Chat(inputText, HandleReply, ReplyCompleted);
        return res;
    }

    void HandleReply(string reply)
    {
        Debug.Log(reply);
    }

    void ReplyCompleted()
    {
        Debug.Log("AI Reply Done");
    }
}