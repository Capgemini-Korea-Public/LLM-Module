using System;
using System.Collections.Generic;
using TMPro;
using Unity.Sentis;
using UnityEngine;

[Serializable]
public struct LLMModel
{
    public string ModelName;
    public ModelAsset ONNXModel;
    public TextAsset Tokenizer;
}

public class SentisManager : MonoBehaviour
{
    [Header("Sentis 모델")]
    public List<LLMModel> Models;
    public int idx = 0;

    private Model runtimeModel;
    private Worker worker;
    private Tokenizer tokenizer;
    private TensorShape shape;
    private Tensor<int> InputTensor;

    private void Start()
    {
        LoadModel();
    }

    private void LoadModel()
    {
        runtimeModel = ModelLoader.Load(Models[idx].ONNXModel);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);
        Debug.Log("Model Load Success");

        tokenizer = new Tokenizer(Models[idx].Tokenizer);
        Debug.Log("Token Ready");
    }
    public void InputFieldRun(TMP_InputField inputField)
    {
        Run(inputField.text);
    }
    public async void Run(string inputText)
    {
        // Tokenizer 적용
        int[] tokenIds = tokenizer.Encode(inputText);
        shape = new TensorShape(1, tokenIds.Length);

        // Tensor 생성 및 데이터 채우기 
        InputTensor = new Tensor<int>(shape);
        InputTensor.Upload(tokenIds);

        // attention_mask 배열 생성 (여기서는 모두 1)
        int[] attentionMaskArray = new int[tokenIds.Length];
        for (int i = 0; i < tokenIds.Length; i++)
        {
            attentionMaskArray[i] = 1;
        }
        // attention_mask 텐서 생성 및 데이터 업로드
        Tensor<int> attentionMaskTensor = new Tensor<int>(shape);
        attentionMaskTensor.Upload(attentionMaskArray);

        // 모델이 두 개의 입력을 받는 경우, 각각의 이름에 맞춰 입력 설정
        worker.SetInput("input_ids", InputTensor);
        worker.SetInput("attention_mask", attentionMaskTensor);
        worker.Schedule();

        Tensor<int> outputTensor = await worker.PeekOutput().ReadbackAndCloneAsync() as Tensor<int>;
        Debug.Log(outputTensor.count);
        int[] outputTokenIds = outputTensor.DownloadToArray();
        string resultText = tokenizer.Decode(outputTokenIds);

        Debug.Log($"Model Output: {resultText}");

        // 메모리 정리
        outputTensor.Dispose();
        InputTensor.Dispose();
        attentionMaskTensor.Dispose();
    }
}
