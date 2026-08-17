#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VoiceHarnessSceneSetup
{
    private const string ScenePath = "Assets/Scenes/_Sandbox/VoiceHarness.unity";

    [MenuItem("Tools/StudyWithMe/Create Voice Harness Scene")]
    public static void CreateScene()
    {
        Directory.CreateDirectory("Assets/Scenes/_Sandbox");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var harnessGo = new GameObject("VoiceHarness");
        var session = harnessGo.AddComponent<GeminiLiveVoiceSession>();
        harnessGo.AddComponent<VoiceHarnessHud>();

        string[] configGuids = AssetDatabase.FindAssets("t:GeminiApiConfig");
        if (configGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            session.config = AssetDatabase.LoadAssetAtPath<GeminiApiConfig>(path);
        }
        else
        {
            Debug.LogWarning("No GeminiApiConfig asset found — assign GeminiLiveVoiceSession.config manually before testing.");
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log("Voice harness scene created at " + ScenePath);
    }
}
#endif
