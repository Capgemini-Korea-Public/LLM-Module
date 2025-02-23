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
                //Ollama�� ��ġ�� model�� ���ٰ� ǥ��
            }
        }
    }

    private async void FetchModelsAndShowDropdown()
    {
        showDropdown = false; // �ε� �߿��� ��Ӵٿ� �����
        EditorUtility.DisplayProgressBar("Fetching Models", "Please wait...", 0.5f);

        try
        {
            localhostModels = await FetchOllamaModels();
            foreach (var item in localhostModels)
            {
                Debug.Log(item);
            }
            showDropdown = true; // �� �ε� �Ϸ� �� ��Ӵٿ� ǥ��
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching models: {e.Message}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            Repaint(); // UI ����
        }
    }

    private async Task<List<string>> FetchOllamaModels()
    {
        // Ollama API�� ���� ��� ������ �� ����Ʈ�� �������� ���� ����
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
