# Unity LLM Module
![Unity](https://img.shields.io/badge/Unity-6000.0.37+-black.svg?style=flat&logo=unity)
![License](https://img.shields.io/badge/License-MIT-blue.svg?style=flat) 
# Feature

Select desired models from various environments such as RESTfulAPI, LocalhostAPI, and Local Library.

Utilize LLM functionality through a unified execution method using "one" component.

# Requirements

## Unity Version
Tested on Unity: Unity 6 


## External Dependencies

- Ollama
- OpenAI Unity
- LLM Unity

# Installation and Usage

## Installation

1. Open Package Manager and select "Add package from git URL".
2. Install LLM Module: https://github.com/Capgemini-Korea-Public/Unity-LLM-Module.git

## Usage

1. Add Component "LLM Module"
   <br>![image](https://github.com/user-attachments/assets/46071f96-6b72-4abe-9760-5560708fdb40)
2. Select API Type
   <br>![image](https://github.com/user-attachments/assets/d4a58619-8cfd-4ccb-b07d-de6a112a889c)

3. Select Model
   
    - LocalLibrary
    1. Press the "Add LLM and LLM Character" Button
    <br>![image](https://github.com/user-attachments/assets/43dd490c-bf49-49b8-acc6-ea43207a1eb0)

    2. choose model or add model at LLM
    <br>![image](https://github.com/user-attachments/assets/0780950d-a905-4225-b4c5-0879b7f48b8d)
    <br> Download one of the default models with the Download Model button (~GBs).
    <br> Or load your own .gguf model with the Load model button 
    <br> more detail : (see [LLM model management](https://github.com/undreamai/LLMUnity?tab=readme-ov-file#llm-model-management)).
 
    - LocalhostAPI
    1. Press the "Fetch Ollama Models)
    <br>![image](https://github.com/user-attachments/assets/134e35cd-cdf7-4815-844d-e50d9509aae3)
    
    2. Select Model (which is installed in Ollama)
    <br>![image](https://github.com/user-attachments/assets/569157c7-0384-42c4-bdcc-201b3f2f4e33)

    
    - RestfulAPI (InProgress)
   
4. Run

# External Dependencies

## Ollama
### Ollama Setup

1. Download and Install [ollama](https://ollama.com/)
2. Pull a model of choice from the [Library](https://ollama.com/library)
    - Recommend `llama3.1` for general conversation
        ```bash
        ollama pull llama3.1
        ```
    - Recommend `gemma2:2b` for device with limited memory
        ```bash
        ollama pull gemma2:2b
        ```
    - Recommend `llava` for image captioning
        ```bash
        ollama pull llava
        ```

## OpenAI Unity

### Importing OpenAI-Unity Package

1. Open Package Manager in Unity (Window > Package Manager)
2. Click the + button and select "Add package from git URL"
3. Paste: `https://github.com/srcnalt/OpenAI-Unity.git` and click Add

### Setting Up OpenAI Account

1. Sign up at https://openai.com/api
2. Generate API key at https://beta.openai.com/account/api-keys

### Saving Credentials

1. Create `.openai` folder in your home directory (e.g. `C:User\UserName\` for Windows)
2. Create `auth.json` file in the `.openai` folder
3. Add API key and organization (if applicable) to `auth.json`:

  ```json
  {
      "api_key": "sk-...W6yi",
      "organization": "org-...L7W"
  }
  ```

**IMPORTANT:** Your API key is a secret. 
Do not share it with others or expose it in any client-side code (e.g. browsers, apps). 
If you are using OpenAI for production, make sure to run it on the server side, where your API key can be securely loaded from an environment variable or key management service.

More Details : https://github.com/srcnalt/OpenAI-Unity/tree/master


### LLM Unity
1.  Open the Package Manager in Unity: `Window > Package Manager`
2.  Click the `+` button and select `Add package from git URL`
3. Use the repository URL `https://github.com/undreamai/LLMUnity.git` and click `Add`

More Details : https://github.com/undreamai/LLMUnity/tree/main
