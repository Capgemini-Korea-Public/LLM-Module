using System.Threading.Tasks;

public interface ILLMService
{
    public void Init();

    // ChatBot with selected LLM
    public Task<string> Chat(string inputText);
}