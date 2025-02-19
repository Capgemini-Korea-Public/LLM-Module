using UnityEngine;
using LLMUnity;
public class GameManager : MonoBehaviour
{
    LLMManager llmManager;
    private void Awake()
    {
        llmManager = new LLMManager();
    }
}
