using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine; // NuGet에서 Newtonsoft.Json 패키지를 추가하세요.

/// <summary> /// JSON 파일의 구조에 맞춘 클래스들 /// </summary> 
public class TokenizerConfig { public string version; public object truncation; public object padding; public List<AddedToken> added_tokens; public object normalizer; public PreTokenizerConfig pre_tokenizer; public PostProcessorConfig post_processor; public DecoderConfig decoder; public ModelConfig model; }
public class AddedToken { public int id; public string content; public bool single_word; public bool lstrip; public bool rstrip; public bool normalized; public bool special; }
public class PreTokenizerConfig { public string type; public bool add_prefix_space; public bool trim_offsets; public bool use_regex; }
public class PostProcessorConfig { public string type; public bool add_prefix_space; public bool trim_offsets; public bool use_regex; }
public class DecoderConfig { public string type; public bool add_prefix_space; public bool trim_offsets; public bool use_regex; }
public class ModelConfig
{
    public string type; public object dropout; public object unk_token; public string continuing_subword_prefix; public string end_of_word_suffix; public bool fuse_unk; public bool byte_fallback; public Dictionary<string, int> vocab; // merges 규칙이 있다면 여기에 추가할 수 있습니다.
}

/// <summary> /// Unity Sentis에서 사용할 수 있는 Tokenizer 클래스 (간단한 ByteLevel BPE 기반 구현 예시) /// </summary> 
public class Tokenizer
{
    private Dictionary<string, int> vocab;
    private Dictionary<int, string> invVocab;
    private int maxTokenLength;

    /// <summary>
    /// JSON 파일 경로를 받아 토크나이저 설정을 초기화합니다.
    /// </summary>
    /// <param name="jsonPath">tokenizer JSON 파일 경로</param>
    public Tokenizer(TextAsset json)
    {
        // JSON 파싱 (Newtonsoft.Json 사용)
        TokenizerConfig config = JsonConvert.DeserializeObject<TokenizerConfig>(json.text);

        // vocab 초기화 (추가 토큰이 있다면 config.added_tokens의 내용을 반영할 수도 있습니다)
        vocab = config.model.vocab;

        // 역방향 매핑: id -> token
        invVocab = vocab.ToDictionary(kv => kv.Value, kv => kv.Key);

        // vocab 내 토큰의 최대 길이를 계산 (최대 매칭을 위해)
        maxTokenLength = vocab.Keys.Max(token => token.Length);
    }

    /// <summary>
    /// 입력 문자열을 토큰 ID 리스트로 인코딩합니다.
    /// (여기서는 간단한 그리디 알고리즘으로 최대 매칭 방식을 사용)
    /// </summary>
    /// <param name="text">인코딩할 텍스트</param>
    /// <returns>토큰 ID 리스트</returns>
    public int[] Encode(string text)
    {
        List<int> tokens = new List<int>();
        int i = 0;
        while (i < text.Length)
        {
            int length = Math.Min(maxTokenLength, text.Length - i);
            bool matchFound = false;
            // 최대 길이부터 1글자까지 감소시키며 vocab에서 매칭되는 토큰을 찾음
            while (length > 0)
            {
                string substr = text.Substring(i, length);
                if (vocab.ContainsKey(substr))
                {
                    tokens.Add(vocab[substr]);
                    i += length;
                    matchFound = true;
                    break;
                }
                length--;
            }
            if (!matchFound)
            {
                // 매칭되는 토큰이 없으면 (이론상 발생하지 않아야 함) 한 글자씩 처리
                tokens.Add(vocab[text.Substring(i, 1)]);
                i++;
            }
        }

        int[] retToken = new int[tokens.Count];
        for (int j = 0; j < tokens.Count; j++)
        {
            retToken[j] = tokens[j];
        }

        return retToken;
    }

    /// <summary>
    /// 토큰 ID 리스트를 다시 문자열로 디코딩합니다.
    /// </summary>
    /// <param name="tokenIds">토큰 ID 리스트</param>
    /// <returns>디코딩된 텍스트</returns>
    public string Decode(int[] tokenIds)
    {
        string text = "";
        foreach (int id in tokenIds)
        {
            if (invVocab.ContainsKey(id))
            {
                text += invVocab[id];
            }
        }
        return text;
    }
}