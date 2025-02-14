using System;
using System.Collections.Generic;
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

    public async void Run(string inputText)
    {
        // Tokenizer 적용
        int[] tokenIds = tokenizer.Encode(inputText);
        Debug.Log(tokenIds.Length);
        shape = new TensorShape(1, tokenIds.Length);

        // Tensor 생성 및 데이터 채우기 
        InputTensor = new Tensor<int>(shape);
        InputTensor.Upload(tokenIds);

        worker.Schedule(InputTensor);
        Tensor<int> outputTensor = await worker.PeekOutput().ReadbackAndCloneAsync() as Tensor<int>;

        int[] outputTokenIds = outputTensor.DownloadToArray();
        string resultText = tokenizer.Decode(outputTokenIds);

        Debug.Log($"Model Output: {resultText}");

        // 메모리 정리
        outputTensor.Dispose();
    }
}
