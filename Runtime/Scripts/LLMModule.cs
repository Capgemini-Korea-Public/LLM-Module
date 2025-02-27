using UnityEngine;
using System.Threading.Tasks;
using LLMUnity;
/// <summary>
/// Using LLMUnity, OpenAI, Ollama All in One Module
/// Using For ChatBot AI
/// </summary>

public class LLMModule : MonoBehaviour
{
    // RESTfulAPI, LocalAPI, NativeLibrary ����
    public EAPIType apiType;

    // Adaptor�� ���⿡ ����
    ILLMService llmService;
    AdaptorData llmServiceData;
    [HideInInspector] public LLM llm;
    [HideInInspector] public LLMCharacter llmCharacter;
    [HideInInspector] public string LocalhostModel;

    public int selectedModelIndex = 0;
    // EModelType�� ���� �ٸ� llmService�� �����ش�.

    private void Start()
    {
        switch(apiType){
            // ���� custom editor�� Ȱ���Ͽ� native�� ����� ���� component�� �߰��ϴ� ������ ����
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

