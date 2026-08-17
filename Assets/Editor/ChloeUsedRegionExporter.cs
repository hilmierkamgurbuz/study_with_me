#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ChloeUsedRegionExporter
{
    private const string OutputDir = "Assets/Art/Character/UsedIslands";

    [MenuItem("Tools/StudyWithMe/Log Chloe Face UV Bounds")]
    public static void LogFaceUvBounds()
    {
        var chloeGo = FindChloeRoot();
        if (chloeGo == null)
        {
            Debug.LogError("ChloeUsedRegionExporter: no CharacterPresenter found in the open scene.");
            return;
        }

        foreach (var r in chloeGo.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (LabelFor(r) != "Face") continue;

            Mesh mesh = r.sharedMesh;
            Vector2[] uvs = mesh != null ? mesh.uv : null;
            if (uvs == null || uvs.Length == 0)
            {
                Debug.LogWarning("ChloeUsedRegionExporter: face mesh has no UV data.");
                return;
            }

            float minU = float.MaxValue, maxU = float.MinValue, minV = float.MaxValue, maxV = float.MinValue;
            foreach (var uv in uvs)
            {
                if (uv.x < minU) minU = uv.x;
                if (uv.x > maxU) maxU = uv.x;
                if (uv.y < minV) minV = uv.y;
                if (uv.y > maxV) maxV = uv.y;
            }

            Debug.Log(string.Format(
                "ChloeUsedRegionExporter: face mesh UV bounds -> U [{0:F4}, {1:F4}] (width {2:F4}), V [{3:F4}, {4:F4}] (height {5:F4})",
                minU, maxU, maxU - minU, minV, maxV, maxV - minV));
            return;
        }

        Debug.LogWarning("ChloeUsedRegionExporter: no Face-labeled SkinnedMeshRenderer found under 'Chloe'.");
    }

    [MenuItem("Tools/StudyWithMe/Export Chloe Used Region")]
    public static void Export()
    {
        var chloeGo = FindChloeRoot();
        if (chloeGo == null)
        {
            Debug.LogError("ChloeUsedRegionExporter: no CharacterPresenter found in the open scene.");
            return;
        }

        Directory.CreateDirectory(OutputDir);

        foreach (var r in chloeGo.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            string label = LabelFor(r);
            if (label == null) continue;
            ProcessRenderer(r, label);
        }

        AssetDatabase.Refresh();
    }

    private static void LogVertexColors(Mesh mesh, string label)
    {
        Color32[] colors = mesh.colors32;
        if (colors == null || colors.Length == 0)
        {
            Debug.Log(string.Format("ChloeUsedRegionExporter: {0} mesh has NO vertex colors (colors32 empty).", label));
            return;
        }

        var counts = new Dictionary<Color32, int>();
        foreach (var c in colors)
        {
            if (counts.ContainsKey(c)) counts[c]++;
            else counts[c] = 1;
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendFormat("ChloeUsedRegionExporter: {0} mesh has {1} vertex(es), {2} distinct vertex color(s):\n", label, colors.Length, counts.Count);
        int shown = 0;
        foreach (var kv in counts)
        {
            if (shown >= 20) { sb.AppendLine("  ... more"); break; }
            sb.AppendFormat("  #{0:X2}{1:X2}{2:X2}{3:X2} : {4} vertex(es)\n", kv.Key.r, kv.Key.g, kv.Key.b, kv.Key.a, kv.Value);
            shown++;
        }
        Debug.Log(sb.ToString());
    }

    private static GameObject FindChloeRoot()
    {
        // name lookup is unreliable here: the character's own body mesh child
        // is named the same as the root ("Chloe"), so GameObject.Find's match
        // among same-named objects is unspecified. CharacterPresenter only
        // ever exists on the actual root, so it can't collide.
        var presenter = Object.FindFirstObjectByType<CharacterPresenter>();
        return presenter != null ? presenter.gameObject : null;
    }

    private static string LabelFor(SkinnedMeshRenderer r)
    {
        foreach (var m in r.sharedMaterials)
        {
            if (m == null) continue;
            string n = m.name.ToLower();
            if (n.Contains("gameroom")) return "Body";
            if (n.Contains("face")) return "Face";
        }
        return null;
    }

    private static Material FindMaterialByLabel(SkinnedMeshRenderer r, string label)
    {
        foreach (var m in r.sharedMaterials)
        {
            if (m == null) continue;
            string n = m.name.ToLower();
            if (label == "Body" && n.Contains("gameroom")) return m;
            if (label == "Face" && n.Contains("face")) return m;
        }
        return null;
    }

    private static void ProcessRenderer(SkinnedMeshRenderer renderer, string label)
    {
        Mesh mesh = renderer.sharedMesh;
        if (mesh == null) return;

        LogVertexColors(mesh, label);

        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;
        if (uvs == null || uvs.Length == 0 || triangles == null || triangles.Length == 0)
        {
            Debug.LogWarning("ChloeUsedRegionExporter: " + label + " mesh has no UV/triangle data.");
            return;
        }

        Material mat = FindMaterialByLabel(renderer, label);
        Texture2D sourceTex = mat != null ? mat.mainTexture as Texture2D : null;
        if (sourceTex == null)
        {
            Debug.LogWarning("ChloeUsedRegionExporter: " + label + " material/texture not found.");
            return;
        }

        string texPath = AssetDatabase.GetAssetPath(sourceTex);
        var importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
        bool wasReadable = importer != null && importer.isReadable;
        if (importer != null && !wasReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        int width = sourceTex.width;
        int height = sourceTex.height;

        bool[,] covered = new bool[width, height];
        for (int t = 0; t < triangles.Length; t += 3)
        {
            Vector2 uv0 = uvs[triangles[t]];
            Vector2 uv1 = uvs[triangles[t + 1]];
            Vector2 uv2 = uvs[triangles[t + 2]];
            RasterizeTriangle(covered, width, height, uv0, uv1, uv2);
        }

        Color[] srcPixels;
        try
        {
            srcPixels = sourceTex.GetPixels();
        }
        finally
        {
            if (importer != null && !wasReadable)
            {
                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }

        var islands = FindIslands(covered, width, height);
        Debug.Log(string.Format("ChloeUsedRegionExporter: {0} renderer -> {1} island(s) found in {2} ({3}x{4})",
            label, islands.Count, texPath, width, height));

        for (int i = 0; i < islands.Count; i++)
        {
            var island = islands[i];
            int w = island.maxX - island.minX + 1;
            int h = island.maxY - island.minY + 1;

            var cropped = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color[] outPixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int srcX = island.minX + x;
                    int srcY = island.minY + y;
                    int srcIdx = srcY * width + srcX;
                    outPixels[y * w + x] = covered[srcX, srcY] ? srcPixels[srcIdx] : new Color(0, 0, 0, 0);
                }
            }
            cropped.SetPixels(outPixels);
            cropped.Apply();

            string fileName = string.Format("Chloe_{0}_Island_{1}.png", label, i);
            string outPath = Path.Combine(OutputDir, fileName).Replace("\\", "/");
            File.WriteAllBytes(outPath, cropped.EncodeToPNG());

            Debug.Log(string.Format("  Island {0}: {1}x{2} px, {3} covered pixel(s) -> {4}",
                i, w, h, island.pixelCount, outPath));
        }
    }

    private class Island
    {
        public int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
        public int pixelCount;
    }

    private static List<Island> FindIslands(bool[,] covered, int width, int height)
    {
        bool[,] visited = new bool[width, height];
        var islands = new List<Island>();
        var stack = new Stack<Vector2Int>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!covered[x, y] || visited[x, y]) continue;

                var island = new Island();
                stack.Clear();
                stack.Push(new Vector2Int(x, y));
                visited[x, y] = true;

                while (stack.Count > 0)
                {
                    var p = stack.Pop();
                    island.pixelCount++;
                    if (p.x < island.minX) island.minX = p.x;
                    if (p.x > island.maxX) island.maxX = p.x;
                    if (p.y < island.minY) island.minY = p.y;
                    if (p.y > island.maxY) island.maxY = p.y;

                    TryPush(stack, visited, covered, width, height, p.x + 1, p.y);
                    TryPush(stack, visited, covered, width, height, p.x - 1, p.y);
                    TryPush(stack, visited, covered, width, height, p.x, p.y + 1);
                    TryPush(stack, visited, covered, width, height, p.x, p.y - 1);
                }

                islands.Add(island);
            }
        }
        return islands;
    }

    private static void TryPush(Stack<Vector2Int> stack, bool[,] visited, bool[,] covered, int width, int height, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        if (visited[x, y] || !covered[x, y]) return;
        visited[x, y] = true;
        stack.Push(new Vector2Int(x, y));
    }

    private static void RasterizeTriangle(bool[,] covered, int width, int height, Vector2 uv0, Vector2 uv1, Vector2 uv2)
    {
        Vector2 a = new Vector2(uv0.x * width, uv0.y * height);
        Vector2 b = new Vector2(uv1.x * width, uv1.y * height);
        Vector2 c = new Vector2(uv2.x * width, uv2.y * height);

        int minX = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.x, b.x, c.x)), 0, width - 1);
        int maxX = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.x, b.x, c.x)), 0, width - 1);
        int minY = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.y, b.y, c.y)), 0, height - 1);
        int maxY = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.y, b.y, c.y)), 0, height - 1);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                if (PointInTriangle(p, a, b, c))
                    covered[x, y] = true;
            }
        }
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = Sign(p, a, b);
        float d2 = Sign(p, b, c);
        float d3 = Sign(p, c, a);
        bool hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
        bool hasPos = d1 > 0 || d2 > 0 || d3 > 0;
        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
#endif
