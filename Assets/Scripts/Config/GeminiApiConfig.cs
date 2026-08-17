using UnityEngine;

[CreateAssetMenu(fileName = "GeminiApiConfig", menuName = "StudyWithMe/Gemini API Config")]
public class GeminiApiConfig : ScriptableObject
{
    public string apiKey;

    // Live API model ids are preview-tier and rotate — re-verify at
    // https://ai.google.dev/gemini-api/docs/models before trusting this.
    public string liveModelId = "models/gemini-3.1-flash-live-preview";
}
