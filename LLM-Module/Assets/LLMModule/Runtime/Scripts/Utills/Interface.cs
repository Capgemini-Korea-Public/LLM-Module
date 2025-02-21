using System.Threading.Tasks;

public interface ILLMService
{
    public void Init(AdaptorData adaptorData);

    // ChatBot with selected LLM
    public Task<string> Chat(string inputText);
}