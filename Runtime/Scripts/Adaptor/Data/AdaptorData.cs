using LLMUnity;

public class AdaptorData
{
    public string modelName;
    public string role;
}
public class LocalhostAdaptorData : AdaptorData
{
    public LocalhostAdaptorData(string modelName)
    {
        this.modelName = modelName;
    }
}

public class RESTfulAdaptorData : AdaptorData
{

}

public class LocalLibraryAdaptorData : AdaptorData
{
    public LLM llm;
    public LLMCharacter llmCharacter;
    public LocalLibraryAdaptorData(LLM llm, LLMCharacter llmCharacter)
    {
        this.llm = llm;
        this.llmCharacter = llmCharacter;
    }
}