using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// One-shot import setup for the vendored Fruit Merge minigame. Everything here is a
// PROJECT SETTINGS change, which is why it is a menu item instead of a YAML edit: the
// Editor holds ProjectSettings in memory and writes them back on quit, so a file edited
// from outside a running Editor is silently overwritten. Going through SerializedObject
// makes Unity itself the writer.
//
// Safe to run more than once — every step is a no-op when the value is already right.
public static class FruitMergeImportSetup
{
    private const string GameScenePath = "Assets/FruitMerge/Scenes/Game.unity";

    // EditorSettings.m_SpritePackerMode. Written as the raw int rather than through the
    // SpritePackerMode enum so a rename in a future Unity version cannot break the build
    // of this file; 5 is what the Fruit Merge project ships (Sprite Atlas V2).
    private const int SpriteAtlasV2 = 5;

    // Layer INDICES are baked into Game.unity and Fruit.prefab, so the names have to land
    // on exactly these slots — a name in the right list but the wrong slot would leave the
    // scene's colliders on unnamed layers. 8 (Room) is not used by the game; game mode
    // needs it so the game's orthographic camera can cull the room away, and TagManager is
    // touched once rather than twice.
    private static readonly (int Index, string Name)[] RequiredLayers =
    {
        (6, "Fruit"),
        (7, "Wall"),
        (8, "Room"),
    };

    [MenuItem("Tools/Fruit Merge/Apply Import Settings")]
    public static void Apply()
    {
        // Non-short-circuiting &= on purpose: every step runs and reports, so one failure
        // does not hide the state of the others.
        bool ok = true;
        ok &= ApplyLayers();
        ok &= ApplySpritePackerMode();
        ok &= ApplyBuildSettings();

        AssetDatabase.SaveAssets();

        if (ok) Debug.Log("[FruitMergeImportSetup] done — layers, sprite packer and build settings are in place.");
        else Debug.LogError("[FruitMergeImportSetup] finished with errors; see the messages above.");
    }

    private static bool ApplyLayers()
    {
        SerializedObject tagManager = LoadSettings("ProjectSettings/TagManager.asset");
        if (tagManager == null) return false;

        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers == null || !layers.isArray)
        {
            Debug.LogError("[FruitMergeImportSetup] TagManager has no 'layers' array; set the layers by hand in Project Settings > Tags and Layers.");
            return false;
        }

        bool ok = true;
        foreach ((int index, string layerName) in RequiredLayers)
        {
            if (index >= layers.arraySize)
            {
                Debug.LogError("[FruitMergeImportSetup] layer slot " + index + " is out of range (" + layers.arraySize + " slots).");
                ok = false;
                continue;
            }

            SerializedProperty slot = layers.GetArrayElementAtIndex(index);
            string current = slot.stringValue;

            if (current == layerName) continue;

            // Never overwrite a name someone else put there: the index is what matters to
            // the scene, so a collision is a real conflict that a human has to resolve.
            if (!string.IsNullOrEmpty(current))
            {
                Debug.LogError("[FruitMergeImportSetup] layer " + index + " is already called '" + current +
                               "'; Fruit Merge needs it to be '" + layerName + "'. Free the slot or remap the game's layers.");
                ok = false;
                continue;
            }

            slot.stringValue = layerName;
            Debug.Log("[FruitMergeImportSetup] layer " + index + " = '" + layerName + "'.");
        }

        tagManager.ApplyModifiedProperties();
        return ok;
    }

    private static bool ApplySpritePackerMode()
    {
        SerializedObject editorSettings = LoadSettings("ProjectSettings/EditorSettings.asset");
        if (editorSettings == null) return false;

        SerializedProperty mode = editorSettings.FindProperty("m_SpritePackerMode");
        if (mode == null)
        {
            Debug.LogError("[FruitMergeImportSetup] EditorSettings has no 'm_SpritePackerMode'; set Sprite Packer to 'Sprite Atlas V2' by hand in Project Settings > Editor.");
            return false;
        }

        if (mode.intValue == SpriteAtlasV2) return true;

        mode.intValue = SpriteAtlasV2;
        editorSettings.ApplyModifiedProperties();

        // The game's two .spriteatlasv2 assets are inert in any other mode — the sprites
        // still render, they just are not batched, so this is a performance fix rather
        // than a correctness one. It triggers a full reimport.
        Debug.Log("[FruitMergeImportSetup] sprite packer set to Sprite Atlas V2 — a reimport will follow.");
        return true;
    }

    private static bool ApplyBuildSettings()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GameScenePath) == null)
        {
            Debug.LogError("[FruitMergeImportSetup] " + GameScenePath + " not found — the Fruit Merge import is incomplete.");
            return false;
        }

        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].path != GameScenePath) continue;
            if (scenes[i].enabled) return true;

            scenes[i] = new EditorBuildSettingsScene(GameScenePath, true);
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[FruitMergeImportSetup] " + GameScenePath + " re-enabled in Build Settings.");
            return true;
        }

        // Appended, never inserted at 0: index 0 is the scene a build boots into, and that
        // stays Room.unity.
        scenes.Add(new EditorBuildSettingsScene(GameScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("[FruitMergeImportSetup] " + GameScenePath + " added to Build Settings.");
        return true;
    }

    private static SerializedObject LoadSettings(string path)
    {
        // Fully qualified: with both UnityEditor and UnityEngine in scope, a bare Object
        // is one using-directive away from meaning something else.
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        if (assets == null || assets.Length == 0 || assets[0] == null)
        {
            Debug.LogError("[FruitMergeImportSetup] could not open " + path + ".");
            return null;
        }

        return new SerializedObject(assets[0]);
    }
}
