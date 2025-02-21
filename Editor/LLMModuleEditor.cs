using LLMUnity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Ollama;

[CustomEditor(typeof(LLMModule), true)]
public class LLMModuleEditor : Editor
{
    private LLMModule targetComponent;
    private EAPIType previousApiType;

    private void OnEnable()
    {
        targetComponent = (LLMModule)target;
        previousApiType = targetComponent.apiType;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("apiType"));

        if (targetComponent.apiType != previousApiType)
        {
            if (previousApiType == EAPIType.Native)
            {
                RemoveComponent<LLM>();
                RemoveComponent<LLMCharacter>();
            }
            previousApiType = targetComponent.apiType;
        }

        switch (targetComponent.apiType)
        {
            case EAPIType.Native:
                HandleNativeType();
                break;
            case EAPIType.LocalhostAPI:
                HandleLocalhostType();
                break;
            case EAPIType.RestfulAPI:
                HandleRestfulType();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void HandleNativeType()
    {
        if (GUILayout.Button("Add LLM and LLMCharacter"))
        {
            targetComponent.llm = AddComponent<LLM>();
            targetComponent.llmCharacter = AddComponent<LLMCharacter>();
        }
    }

    private async void HandleLocalhostType()
    {
        if (GUILayout.Button("Fetch Ollama Models"))
        {

            List<string> models = await FetchOllamaModels();
            // 모델 리스트를 표시하거나 저장하는 로직 구현
        }
    }

    private void HandleRestfulType()
    {
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("apiKey"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("model"));
    }


    private async Task<List<string>> FetchOllamaModels()
    {
        // Ollama API를 통해 사용 가능한 모델 리스트를 가져오는 로직 구현
        Model[] models = await List();
        List<string> modelNames = new List<string>();
        foreach (Model model in models) 
        {
            modelNames.Add(model.name);
        }
        return modelNames;
    }

    private T AddComponent<T>() where T : Component
    {
        if (targetComponent.GetComponent<T>() == null)
            return targetComponent.gameObject.AddComponent<T>();
        
        else
            return targetComponent.GetComponent<T>();
    }

    private void RemoveComponent<T>() where T : Component
    {
        T component = targetComponent.GetComponent<T>();
        if (component != null)
        {
            DestroyImmediate(component);
        }
    }
}
