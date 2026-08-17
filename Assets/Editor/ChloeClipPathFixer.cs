#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ChloeClipPathFixer
{
    // The hierarchy every generated clip is bound against. 29 of the 30
    // Chloe_Rigged@*.fbx clips hold a single scene-root node, so Unity makes that
    // node the imported root and their curve paths come out as "spine/..." — while
    // this model has three root nodes (Armature, Chloe, Chloe_Face) and therefore
    // uses "Armature/spine/...". The paths never bind, so the state drives nothing
    // and WriteDefaultValues snaps every bone to the standing rest pose.
    private const string ModelPath = "Assets/Art/Character/deneme.fbx";
    private const string OutputDir = "Assets/Art/chloe/Generated";

    // The rig's root-motion node. Curves targeting it are dropped so every
    // generated clip is strictly in-place. This has to apply to all clips at once:
    // "Sitting Talking" carries its baked seated offset at the empty path (it *is*
    // the clip root there) and "Sitting Idle" at "Armature", and the two disagree
    // by 8 cm in Y and 23 cm in Z — dropping one while keeping the other would
    // turn that lurch into a 30 cm one.
    private const string RootMotionNode = "Armature";

    [MenuItem("Tools/StudyWithMe/Rebind Clip Paths To Model")]
    public static void Rebind()
    {
        HashSet<string> modelPaths = ModelTransformPaths();
        if (modelPaths.Count == 0)
        {
            Debug.LogError("ChloeClipPathFixer: could not read a transform hierarchy from " + ModelPath);
            return;
        }

        List<AnimationClip> sources = SelectedClips();
        if (sources.Count == 0)
        {
            Debug.LogWarning("ChloeClipPathFixer: select one or more FBX assets that contain animation clips first.");
            return;
        }

        EnsureFolder(OutputDir);

        int written = 0;
        foreach (AnimationClip clip in sources)
        {
            if (Generate(clip, modelPaths)) written++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(string.Format("ChloeClipPathFixer: wrote {0} of {1} clip(s) into {2}",
            written, sources.Count, OutputDir));
    }

    // AssetDatabase.CreateAsset needs the folder to exist *in the database*, which
    // creating it on disk alone does not achieve until the next refresh
    private static void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder)) return;

        int split = folder.LastIndexOf('/');
        if (split <= 0) return;

        string parent = folder.Substring(0, split);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, folder.Substring(split + 1));
    }

    private static HashSet<string> ModelTransformPaths()
    {
        var paths = new HashSet<string>();
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (model == null) return paths;

        foreach (Transform t in model.GetComponentsInChildren<Transform>(true))
        {
            string path = AnimationUtility.CalculateTransformPath(t, model.transform);
            if (!string.IsNullOrEmpty(path)) paths.Add(path);
        }
        return paths;
    }

    private static List<AnimationClip> SelectedClips()
    {
        var clips = new List<AnimationClip>();
        foreach (UnityEngine.Object selected in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(selected);
            if (string.IsNullOrEmpty(assetPath)) continue;

            foreach (UnityEngine.Object sub in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                var clip = sub as AnimationClip;
                if (clip == null) continue;
                // the importer's hidden preview clip is not a real take
                if (clip.name.StartsWith("__preview__", StringComparison.Ordinal)) continue;
                if (!clips.Contains(clip)) clips.Add(clip);
            }
        }
        return clips;
    }

    private static bool Generate(AnimationClip source, HashSet<string> modelPaths)
    {
        var rebound = new AnimationClip { frameRate = source.frameRate };
        int matched = 0, remapped = 0, droppedRoot = 0;
        var unresolved = new List<string>();

        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(source))
        {
            string path;
            if (!Route(binding.path, modelPaths, out path, ref droppedRoot, unresolved)) continue;
            if (path == binding.path) matched++; else remapped++;
            AnimationUtility.SetEditorCurve(rebound, Retarget(binding, path),
                AnimationUtility.GetEditorCurve(source, binding));
        }

        foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(source))
        {
            string path;
            if (!Route(binding.path, modelPaths, out path, ref droppedRoot, unresolved)) continue;
            if (path == binding.path) matched++; else remapped++;
            AnimationUtility.SetObjectReferenceCurve(rebound, Retarget(binding, path),
                AnimationUtility.GetObjectReferenceCurve(source, binding));
        }

        if (matched + remapped == 0)
        {
            Debug.LogError(string.Format(
                "ChloeClipPathFixer: '{0}' produced no bindings that resolve against {1} — nothing written.",
                source.name, ModelPath));
            UnityEngine.Object.DestroyImmediate(rebound);
            return false;
        }

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(source);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(rebound, settings);

        string outputPath = OutputDir + "/" + source.name + ".anim";
        var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);
        if (existing != null)
        {
            // overwrite in place so a re-run keeps the guid, and every reference
            // already made from the controller survives
            EditorUtility.CopySerialized(rebound, existing);
            EditorUtility.SetDirty(existing);
            UnityEngine.Object.DestroyImmediate(rebound);
        }
        else
        {
            AssetDatabase.CreateAsset(rebound, outputPath);
        }

        Debug.Log(string.Format(
            "ChloeClipPathFixer: '{0}' -> {1}  ({2} remapped, {3} already matched, {4} root-motion curve(s) dropped, loopTime on)",
            source.name, outputPath, remapped, matched, droppedRoot));

        if (unresolved.Count > 0)
        {
            Debug.LogWarning(string.Format(
                "ChloeClipPathFixer: '{0}' had {1} path(s) with no counterpart in the model — those curves were left out: {2}",
                source.name, unresolved.Count, string.Join(", ", unresolved.ToArray())));
        }
        return true;
    }

    private static bool Route(string sourcePath, HashSet<string> modelPaths, out string resolved,
        ref int droppedRoot, List<string> unresolved)
    {
        resolved = null;

        // in a single-root clip the root-motion node *is* the clip root, so it
        // shows up as the empty path; in a multi-root clip it shows up by name
        if (string.IsNullOrEmpty(sourcePath) || sourcePath == RootMotionNode)
        {
            droppedRoot++;
            return false;
        }

        if (TryResolve(sourcePath, modelPaths, out resolved)) return true;

        if (!unresolved.Contains(sourcePath)) unresolved.Add(sourcePath);
        return false;
    }

    // A clip path that the model does not have is assumed to be missing leading
    // ancestors, so the model path that ends with it is the intended target. Only
    // an unambiguous single match is accepted — anything else is reported rather
    // than guessed at.
    private static bool TryResolve(string path, HashSet<string> modelPaths, out string resolved)
    {
        resolved = null;
        if (modelPaths.Contains(path))
        {
            resolved = path;
            return true;
        }

        string suffix = "/" + path;
        int hits = 0;
        foreach (string candidate in modelPaths)
        {
            if (!candidate.EndsWith(suffix, StringComparison.Ordinal)) continue;
            resolved = candidate;
            hits++;
        }

        if (hits == 1) return true;
        resolved = null;
        return false;
    }

    private static EditorCurveBinding Retarget(EditorCurveBinding binding, string path)
    {
        return new EditorCurveBinding
        {
            path = path,
            type = binding.type,
            propertyName = binding.propertyName
        };
    }
}
#endif
