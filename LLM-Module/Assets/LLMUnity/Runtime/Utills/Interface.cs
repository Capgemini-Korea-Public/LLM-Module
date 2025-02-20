// ILLMService defines a common interface for LLM-related functionalities
using LLMUnity;
using System.Threading.Tasks;

public interface ILLMService
{
    public Task<Ret> PostRequest<Res, Ret>(string json, string endpoint, ContentCallback<Res, Ret> getContent, Callback<Ret> callback = null);
}
