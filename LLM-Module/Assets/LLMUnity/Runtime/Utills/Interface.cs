// ILLMService defines a common interface for LLM-related functionalities
using LLMUnity;
using System.Threading.Tasks;

public interface ILLMService
{
    // Converts the input text into a list of tokens.
    Task<string> Tokenize(string text);

    // Converts a list of tokens back into the original text.
    Task<string> Detokenize(string tokens);

    // Computes the embedding vector for the input text.
    Task<string> Embeddings(string text);

    // Performs text completion based on the provided prompt.
    // The 'stream' parameter indicates whether the response should be streamed.
    Task<string> Complete(string prompt, Callback<string> streamCallback = null);

    // Handles slot-related operations, such as saving or restoring state.
    Task<string> Slot(string json);
}
