using LLMUnity;
using System;
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
    private bool showDropdown = false;
    List<string> localhostModels = new List<string>();
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

    private void HandleLocalhostType()
    {
        
        if (GUILayout.Button("Fetch Ollama Models"))
        {
            FetchModelsAndShowDropdown();
        }

        if (showDropdown)
        {
            if(localhostModels.Count > 0)
            {
                EditorGUI.indentLevel++;
                targetComponent.selectedModelIndex = EditorGUILayout.Popup("Select Model", targetComponent.selectedModelIndex, localhostModels.ToArray());
                targetComponent.LocalhostModel = localhostModels[targetComponent.selectedModelIndex];
                EditorGUI.indentLevel--;
            }
            else
            {
                //Ollama에 설치된 model이 없다고 표시
            }
        }
    }

    private async void FetchModelsAndShowDropdown()
    {
        showDropdown = false; // 로딩 중에는 드롭다운 숨기기
        EditorUtility.DisplayProgressBar("Fetching Models", "Please wait...", 0.5f);

        try
        {
            localhostModels = await FetchOllamaModels();
            foreach (var item in localhostModels)
            {
                Debug.Log(item);
            }
            showDropdown = true; // 모델 로딩 완료 후 드롭다운 표시
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching models: {e.Message}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            Repaint(); // UI 갱신
        }
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

    private void HandleRestfulType()
    {
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("apiKey"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("model"));
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
