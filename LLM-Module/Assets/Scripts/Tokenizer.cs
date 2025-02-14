using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Tokenizer
{
    private Dictionary<string, int> vocab;      // 단어 → 토큰 ID 매핑
    private Dictionary<int, string> idToToken; // 토큰 ID → 단어 매핑
    private Dictionary<(string, string), int> merges; // BPE 병합 규칙

    public Tokenizer(TextAsset tokenizerJson)
    {
        LoadTokenizer(tokenizerJson);
    }

    private void LoadTokenizer(TextAsset tokenizerJson)
    {
        try
        {
            var tokenizerData = JsonConvert.DeserializeObject<TokenizerData>(tokenizerJson.text);

            vocab = tokenizerData.model.vocab;
            idToToken = vocab.ToDictionary(kv => kv.Value, kv => kv.Key);

            merges = new Dictionary<(string, string), int>();
            for (int i = 0; i < tokenizerData.model.merges.Count; i++)
            {
                string[] mergePair = tokenizerData.model.merges[i].Split(' ');
                merges[(mergePair[0], mergePair[1])] = i;
            }

            Debug.Log("Tokenizer loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load tokenizer: {e.Message}");
        }
    }

    public int[] Encode(string text)
    {
        List<int> tokenIds = new List<int>();

        // Step 1: 문장을 개별 문자로 분해
        List<string> subwords = text.Select(c => c.ToString()).ToList();

        // Step 2: BPE 병합 적용
        while (subwords.Count > 1)
        {
            (string, string)? bestPair = null;
            int bestRank = int.MaxValue;
            int mergeIndex = -1;

            for (int i = 0; i < subwords.Count - 1; i++)
            {
                var pair = (subwords[i], subwords[i + 1]);

                if (merges.TryGetValue(pair, out int rank) && rank < bestRank)
                {
                    bestPair = pair;
                    bestRank = rank;
                    mergeIndex = i;
                }
            }

            if (bestPair == null || mergeIndex == -1)
                break;

            // 가장 우선순위 높은 병합을 수행
            subwords[mergeIndex] = bestPair.Value.Item1 + bestPair.Value.Item2;
            subwords.RemoveAt(mergeIndex + 1);
        }

        // Step 3: vocab을 참고하여 토큰 ID로 변환
        foreach (var token in subwords)
        {
            if (vocab.ContainsKey(token))
            {
                tokenIds.Add(vocab[token]);
            }
            else
            {
                tokenIds.Add(vocab.ContainsKey("<|endoftext|>") ? vocab["<|endoftext|>"] : -1); // 알 수 없는 단어 처리
            }
        }

        return tokenIds.ToArray();
    }

    public string Decode(int[] tokenIds)
    {
        if (tokenIds == null || tokenIds.Length == 0)
            return "";

        return string.Join("", tokenIds.Select(id => idToToken.ContainsKey(id) ? idToToken[id] : "<unk>"));
    }

    private class TokenizerData
    {
        public Model model { get; set; }
    }

    private class Model
    {
        public Dictionary<string, int> vocab { get; set; }
        public List<string> merges { get; set; }
    }
}
