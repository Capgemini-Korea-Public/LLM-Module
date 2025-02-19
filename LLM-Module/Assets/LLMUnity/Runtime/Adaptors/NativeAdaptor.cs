// NativeAdapter wraps the internal LLM instance and implements the ILLMService interface.
// This adapter uses the native (internal) LLM execution method (e.g., gguf model) provided by LLM.cs.
using LLMUnity;
using System.Collections.Generic;
using System.Threading.Tasks;
public class NativeAdapter : ILLMService
{
    private LLM _llm;

    // Constructor that initializes the adapter with an internal LLM instance.
    public NativeAdapter(LLM llm)
    {
        _llm = llm;
    }

    // Uses the internal LLM instance to tokenize the input text.
    public async Task<string> Tokenize(string text)
    {
        return await _llm.Tokenize(text);
    }

    // Uses the internal LLM instance to convert a list of tokens back to text.
    public async Task<string> Detokenize(string tokens)
    {
        return await _llm.Detokenize(tokens);
    }

    // Uses the internal LLM instance to compute embeddings for the input text.
    public async Task<string> Embeddings(string text)
    {
        return await _llm.Embeddings(text);
    }

    // Uses the internal LLM instance to perform text completion based on the given prompt.
    // The 'stream' parameter indicates if the response should be streamed.
    public async Task<string> Complete(string prompt, Callback<string> streamCallback = null)
    {
        return await _llm.Completion(prompt, streamCallback);
    }

    // Uses the internal LLM instance to handle slot operations (e.g., saving/restoring state).
    public async Task<string> Slot(string json)
    {
        return await _llm.Slot(json);
    }
}
