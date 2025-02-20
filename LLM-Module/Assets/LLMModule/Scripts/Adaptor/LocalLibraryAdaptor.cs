using System.Threading.Tasks;

public class LocalLibraryAdaptor : ILLMService
{
    public void Init()
    {
        Ollama.InitChat();
    }

    public Task Chat(string inputText)
    {
        throw new System.NotImplementedException();
    }

    Task<string> ILLMService.Chat(string inputText)
    {
        throw new System.NotImplementedException();
    }
}