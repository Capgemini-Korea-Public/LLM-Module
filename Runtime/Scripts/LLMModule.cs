using UnityEngine;
using System.Threading.Tasks;
using LLMUnity;
/// <summary>
/// Using LLMUnity, OpenAI, Ollama All in One Module
/// Using For ChatBot AI
/// </summary>

public class LLMModule : MonoBehaviour
{
    // RESTfulAPI, LocalAPI, NativeLibrary 선택
    public EAPIType apiType;

    // Adaptor를 여기에 적용
    ILLMService llmService;
    AdaptorData llmServiceData;
    [HideInInspector] public LLM llm;
    [HideInInspector] public LLMCharacter llmCharacter;
    [HideInInspector] public string LocalhostModel;

    public int selectedModelIndex = 0;
    // EModelType에 따라 다른 llmService를 끼워준다.

    private void Start()
    {
        switch(apiType){
            // 차후 custom editor를 활용하여 native를 골랐을 때만 component를 추가하는 것으로 수정
            case EAPIType.Native:
                llmService = new LocalLibraryAdaptor();
                llmServiceData = new LocalLibraryAdaptorData(llm, llmCharacter);
                break;
            case EAPIType.LocalhostAPI:
                llmService = new LocalhostAPIAdaptor();
                llmServiceData = new LocalhostAdaptorData(LocalhostModel);
                break;
            case EAPIType.RestfulAPI:
                llmService = new RESTfulAPIAdaptor();
                llmServiceData = new RESTfulAdaptorData();
                break;
        }

        llmService.Init(llmServiceData);
    }

    public async Task<string> Chat(string inputText)
    {
        Debug.Log($"{llmService.GetType().Name} start");
        return await llmService.Chat(inputText);
    }

    public async Task<string> Chat(string inputText, Texture2D inputImage)
    {
        Debug.Log($"{llmService.GetType().Name} start");
        return await llmService.Chat(inputText, inputImage);
    }
}

