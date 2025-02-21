using LLMUnity;

public class AdaptorData
{
    string modelName;
    string role;
}

public class LocalhostAdaptorData : AdaptorData
{

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