using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Puts the page textures on the book, and MEASURES the book while it is at it.
//
// This tool got the target wrong twice, and both mistakes are worth keeping written
// down because they are the reason it looks the way it does now.
//
// FIRST: it wrote material slot 0. A page block is two submeshes — the paper face and
// the bound edge — and this model orders the paper SECOND, so the image went onto the
// edges.
//
// SECOND, after switching to reading the model's own material names: a plain
// "contains page" match hit M_Book_PageEdge before M_Book_Pages_L and painted exactly
// the same edge again.
//
// And the measurement did NOT save either attempt, which is the part worth
// remembering: the edge submesh spans U 0.003..0.997 / V 0.003..0.886 while the paper
// spans a clean 0..1, so BOTH fill the square. A UV span says whether a submesh can
// carry a full image; it never says whether that submesh is the one you read. Only
// the material name distinguishes them, and only when read carefully — see
// PickPageSlot. The report still measures, because a span that does NOT fill the
// square rules a submesh out, and because a mesh with no UVs at all is worth seeing.
public static class BookPageSetup
{
    private const string MaterialFolder = "Assets/Art/material";
    private const string LeftChildName = "PageBlock_L";
    private const string RightChildName = "PageBlock_R";
    // The single loose sheet caught mid-turn. It lies ON one of the blocks and hides
    // that block's face, so leaving it undressed makes one side look blank however
    // correctly the block underneath was painted.
    private const string FlipChildName = "Page_Flip";
    private const string SpineChildName = "Spine";
    private const string HingeName = "PageHinge";
    private const string LeftTexturePath = "Assets/Art/Textures/book_page.png";
    private const string RightTexturePath = "Assets/Art/Textures/book_page_2.png";

    [MenuItem("Tools/StudyWithMe/Set Up Book Pages")]
    public static void SetUp()
    {
        GameObject book = FindBook();

        if (book == null)
        {
            Debug.LogError("[BookPages] could not find the book: no object in the open scene has a '" +
                           LeftChildName + "' or '" + RightChildName + "' under it. Select the book in " +
                           "the Hierarchy and run this again.");
            return;
        }

        Material left = CreateOrLoadMaterial("book_page_L", LeftTexturePath);
        Material right = CreateOrLoadMaterial("book_page_R", RightTexturePath);

        if (left == null || right == null) return;

        int leftSlot = Assign(book, LeftChildName, left);
        int rightSlot = Assign(book, RightChildName, right);
        int flipSlot = AssignFlipPage(book, left, right);

        // The turning sheet is seen from underneath for the second half of its swing.
        MakeDoubleSided(left);
        MakeDoubleSided(right);

        SetUpTurner(book, left, right, leftSlot, rightSlot, flipSlot);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(book.scene);

        Report(book);

        // Deliberately NOT saved here: this scene has unsaved work in it already, and
        // saving someone else's scene behind their back is a surprise, not a service.
        Debug.Log("[BookPages] done on '" + book.name + "'. The scene is marked dirty — press Cmd+S " +
                  "to keep it. Read the report above before trusting the result.", book);
    }

    /// <summary>
    /// The book is whichever root has a page block somewhere under it. Found by
    /// structure rather than by name because the object was dropped into the scene by
    /// hand and its name is whatever the drop produced. Selection is the fallback.
    /// </summary>
    private static GameObject FindBook()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] roots = scene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            if (FindDescendant(roots[i].transform, LeftChildName) != null ||
                FindDescendant(roots[i].transform, RightChildName) != null)
                return roots[i];
        }

        if (Selection.activeGameObject != null)
        {
            Debug.LogWarning("[BookPages] no '" + LeftChildName + "' found in the scene; falling back to " +
                             "the selected object '" + Selection.activeGameObject.name + "'.");
            return Selection.activeGameObject;
        }

        return null;
    }

    // Includes the transform itself: the page block may BE the object dragged in,
    // rather than a child of it.
    private static Transform FindDescendant(Transform t, string childName)
    {
        if (t.name == childName) return t;

        for (int i = 0; i < t.childCount; i++)
        {
            Transform found = FindDescendant(t.GetChild(i), childName);
            if (found != null) return found;
        }

        return null;
    }

    private static Material CreateOrLoadMaterial(string materialName, string texturePath)
    {
        string path = MaterialFolder + "/" + materialName + ".mat";

        // Never rebuilt: once it has been tuned by hand that material IS the authored
        // look, and a re-run of this tool must not throw the tuning away.
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
        {
            Debug.Log("[BookPages] reusing the existing " + path + ".", existing);
            return existing;
        }

        if (!AssetDatabase.IsValidFolder(MaterialFolder))
        {
            Debug.LogError("[BookPages] " + MaterialFolder + " does not exist.");
            return null;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            Debug.LogError("[BookPages] the URP/Lit shader was not found — is this still a URP project?");
            return null;
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

        if (texture == null)
            Debug.LogWarning("[BookPages] no texture at " + texturePath + "; the material is created blank.");

        Material material = new Material(shader);
        material.SetTexture("_BaseMap", texture);
        // Paper, not plastic: URP/Lit defaults to 0.5 smoothness, which puts a sheen
        // across the page under the room's spot lights.
        material.SetFloat("_Smoothness", 0.1f);
        material.SetFloat("_Metallic", 0f);

        AssetDatabase.CreateAsset(material, path);
        Debug.Log("[BookPages] created " + path + ".", material);

        return material;
    }

    /// <summary>
    /// Writes the slot the MODEL ITSELF calls the pages, and puts every other slot
    /// back to what the FBX shipped with.
    ///
    /// Rather than swap one hardcoded index for another, the target comes from
    /// PickPageSlot reading the FBX's own material names — the model telling us
    /// instead of us guessing. Restoring the other slots from that same source is
    /// what lets a re-run REPAIR an earlier wrong guess instead of layering a second
    /// one on top; both of this tool's mis-assignments were undone that way.
    /// </summary>
    private static int Assign(GameObject book, string childName, Material material)
    {
        Renderer renderer = FindRenderer(book, childName);

        if (renderer == null)
        {
            Debug.LogWarning("[BookPages] no '" + childName + "' with a Renderer under '" + book.name + "'; skipped.");
            return -1;
        }

        return AssignTo(renderer, childName, material);
    }

    /// <returns>The slot written, or -1. BookPageTurner needs it: swapping page
    /// images at runtime means knowing which submesh is the paper, and working that
    /// out once here beats re-deriving it on every turn.</returns>
    private static int AssignTo(Renderer renderer, string childName, Material material)
    {
        Transform child = renderer.transform;

        // The renderer on the FBX asset itself, which still carries the untouched
        // material list however many times this tool has run over the instance.
        Renderer source = PrefabUtility.GetCorrespondingObjectFromSource(renderer) as Renderer;
        Material[] original = source != null ? source.sharedMaterials : null;
        Material[] current = renderer.sharedMaterials;

        if (original == null || original.Length != current.Length)
        {
            Debug.LogError("[BookPages] '" + childName + "' is not an instance of the model (or its slot " +
                           "count changed), so the original material names cannot be read and the page slot " +
                           "cannot be identified. Assign " + material.name + " by hand to the slot whose " +
                           "material is named like M_Book_Pages_*.", child);
            return -1;
        }

        int target = PickPageSlot(original, childName);

        if (target < 0)
        {
            Debug.LogError("[BookPages] none of '" + childName + "'s model materials is named like a page (" +
                           DescribeSlots(original) + "), so there is nothing to be confident about here. " +
                           "Assign " + material.name + " by hand.", child);
            return -1;
        }

        Undo.RecordObject(renderer, "Assign book page material");

        for (int i = 0; i < current.Length; i++) current[i] = i == target ? material : original[i];

        renderer.sharedMaterials = current;
        EditorUtility.SetDirty(renderer);

        Debug.Log("[BookPages] '" + childName + "' slot " + target + " (model called it '" +
                  original[target].name + "') = " + material.name +
                  "; the other slot(s) restored to the model's own materials.", child);

        return target;
    }

    /// <summary>
    /// Which material slot is the paper you read.
    ///
    /// Measured UV cannot answer this and it was checked: on this model the edge
    /// submesh spans U 0.003..0.997 / V 0.003..0.886 and the paper spans a clean
    /// 0..1, so BOTH fill the square and the span tells them apart not at all. The
    /// name is the only real signal, and it has to be read carefully — a plain
    /// "contains page" match picks M_Book_PageEdge, which is exactly the edge that
    /// got painted twice before this function existed.
    ///
    /// So: rule out the parts that are never a reading face whatever they are called,
    /// then prefer the plural. "M_Book_Pages_L" contains "pages"; "M_Book_PageEdge"
    /// does not.
    /// </summary>
    private static int PickPageSlot(Material[] original, string childName)
    {
        int best = -1;
        int bestScore = 0;
        int tied = 0;

        for (int i = 0; i < original.Length; i++)
        {
            if (original[i] == null) continue;

            string name = original[i].name.ToLowerInvariant();

            if (name.Contains("edge") || name.Contains("trim") ||
                name.Contains("spine") || name.Contains("cover")) continue;

            int score = name.Contains("pages") ? 2 : (name.Contains("page") ? 1 : 0);

            if (score == 0) continue;

            if (score > bestScore) { bestScore = score; best = i; tied = 1; }
            else if (score == bestScore) tied++;
        }

        if (tied > 1)
            Debug.LogWarning("[BookPages] '" + childName + "' has " + tied + " slots that look equally like the " +
                             "page face (" + DescribeSlots(original) + "); taking the first. Check the result.");

        return best;
    }

    /// <summary>
    /// Dresses the loose turning sheet with whichever side's texture it is lying on.
    ///
    /// Which side that is gets MEASURED, not assumed: the sheet's bounds centre is
    /// compared against both page blocks' and the nearer one wins. Its material name
    /// (M_Book_Pages_R) hints at the right, but a material name says which surface a
    /// modeller reused, not where the sheet ended up in this pose — and the pose is
    /// what decides which block it hides.
    ///
    /// Without this the covered side reads as a blank page no matter how correctly
    /// the block beneath was painted, which is exactly how it looked.
    /// </summary>
    private static int AssignFlipPage(GameObject book, Material left, Material right)
    {
        Renderer flip = FindRenderer(book, FlipChildName);

        if (flip == null) return -1;

        Renderer leftBlock = FindRenderer(book, LeftChildName);
        Renderer rightBlock = FindRenderer(book, RightChildName);

        if (leftBlock == null || rightBlock == null)
        {
            Debug.LogWarning("[BookPages] '" + FlipChildName + "' found but a page block is missing, so " +
                             "there is nothing to measure it against; skipped.", flip);
            return -1;
        }

        float toLeft = Vector3.Distance(flip.bounds.center, leftBlock.bounds.center);
        float toRight = Vector3.Distance(flip.bounds.center, rightBlock.bounds.center);

        bool onLeft = toLeft <= toRight;

        Debug.Log("[BookPages] '" + FlipChildName + "' sits " + toLeft.ToString("F3") + " from the left block and " +
                  toRight.ToString("F3") + " from the right, so it is lying on the " +
                  (onLeft ? "LEFT" : "RIGHT") + " page.", flip);

        return AssignTo(flip, FlipChildName, onLeft ? left : right);
    }

    // The sheet is seen from its back through the second half of a turn, and a
    // single-sided page simply vanishes there. URP/Lit's Render Face lives in _Cull:
    // 0 is Off, which draws both faces. The back shows the same image mirrored — a
    // genuinely different reverse would need a second sheet back to back, which is
    // more geometry than "there is writing on the turning page" is worth.
    private static void MakeDoubleSided(Material material)
    {
        if (material == null || !material.HasProperty("_Cull")) return;
        if (Mathf.Approximately(material.GetFloat("_Cull"), 0f)) return;

        material.SetFloat("_Cull", 0f);
        material.doubleSidedGI = true;
        EditorUtility.SetDirty(material);

        Debug.Log("[BookPages] " + material.name + " is now double-sided.", material);
    }

    /// <summary>
    /// The line the sheet swings about, derived from the SPINE's measured bounds
    /// rather than typed in: the binding is where a page is hinged, and the spine is
    /// the part that models the binding.
    ///
    /// Its longest bounds axis is the spine's run, and the hinge is raised to the
    /// page plane (the flip sheet's own height) because the sheet is bound at the top
    /// of the spine, not through its middle.
    ///
    /// Never repositioned once it exists: like the zoom camera, a hinge that has been
    /// nudged by hand IS the authored answer.
    /// </summary>
    private static Transform FindOrCreateHinge(GameObject book)
    {
        Transform existing = FindDescendant(book.transform, HingeName);

        if (existing != null)
        {
            // Said out loud rather than kept silently: the rule protects a hinge that
            // was nudged by hand, but it also means a wrongly-measured one stays wrong
            // until someone knows the way out of it.
            Debug.Log("[BookPages] reusing the existing " + HingeName + " — delete it and run again " +
                      "to have it re-measured from scratch.", existing);
            return existing;
        }

        Renderer spine = FindRenderer(book, SpineChildName);

        if (spine == null)
        {
            Debug.LogWarning("[BookPages] no '" + SpineChildName + "' to measure a hinge from; " +
                             "create an empty called '" + HingeName + "' on the book's spine and " +
                             "assign it to BookPageTurner by hand.");
            return null;
        }

        GameObject go = new GameObject(HingeName);
        Undo.RegisterCreatedObjectUndo(go, "Create " + HingeName);
        go.transform.SetParent(book.transform, true);

        Vector3 axis = SpineDirection(spine);
        Vector3 position = SpineCentre(spine);

        Renderer flip = FindRenderer(book, FlipChildName);
        if (flip != null) position.y = flip.bounds.center.y;

        go.transform.position = position;
        // LookRotation's up must not be parallel to the axis it is given.
        Vector3 up = Mathf.Abs(Vector3.Dot(axis, book.transform.up)) > 0.99f ? book.transform.forward : book.transform.up;
        go.transform.rotation = Quaternion.LookRotation(axis, up);

        Debug.Log("[BookPages] created " + HingeName + " at " + position.ToString("F3") +
                  " along " + axis.ToString("F3") + " (measured from the spine's own mesh, not its world box).", go);

        return go.transform;
    }

    /// <summary>
    /// The direction the spine runs, in world space.
    ///
    /// Taken from the LOCAL mesh bounds and then turned by the object's rotation —
    /// NOT from Renderer.bounds. Renderer.bounds is an axis-aligned box in WORLD
    /// space, so for anything rotated its longest side points along a world axis
    /// rather than along the object, and the first version of this picked world Z for
    /// a book sitting at an angle on the desk. The hinge came out crossing the pages
    /// diagonally and the sheet turned corner-to-corner. Local bounds do not move
    /// when the object turns, which is exactly the property needed here.
    /// </summary>
    private static Vector3 SpineDirection(Renderer spine)
    {
        Mesh mesh = MeshOf(spine);

        if (mesh == null) return spine.transform.forward;

        Vector3 size = mesh.bounds.size;

        Vector3 local = size.x >= size.y && size.x >= size.z
            ? Vector3.right
            : (size.z >= size.y ? Vector3.forward : Vector3.up);

        return spine.transform.TransformDirection(local).normalized;
    }

    private static Vector3 SpineCentre(Renderer spine)
    {
        Mesh mesh = MeshOf(spine);

        return mesh != null ? spine.transform.TransformPoint(mesh.bounds.center) : spine.bounds.center;
    }

    private static Mesh MeshOf(Renderer renderer)
    {
        MeshFilter filter = renderer.GetComponent<MeshFilter>();

        if (filter != null && filter.sharedMesh != null) return filter.sharedMesh;

        SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;

        return skinned != null ? skinned.sharedMesh : null;
    }

    private static void SetUpTurner(GameObject book, Material left, Material right,
                                    int leftSlot, int rightSlot, int flipSlot)
    {
        BookPageTurner turner = book.GetComponent<BookPageTurner>();

        if (turner == null)
        {
            turner = Undo.AddComponent<BookPageTurner>(book);
            Debug.Log("[BookPages] added BookPageTurner to '" + book.name + "'.", book);
        }

        Undo.RecordObject(turner, "Wire BookPageTurner");

        turner.leftPage = FindRenderer(book, LeftChildName);
        turner.rightPage = FindRenderer(book, RightChildName);
        turner.flipPage = FindRenderer(book, FlipChildName);
        turner.hinge = FindOrCreateHinge(book);
        turner.pageA = left;
        turner.pageB = right;

        // Slots come from what the assignment above actually did; -1 means it could
        // not be resolved, and the field's own default is left alone rather than
        // being overwritten with a value known to be wrong.
        if (leftSlot >= 0) turner.leftPageSlot = leftSlot;
        if (rightSlot >= 0) turner.rightPageSlot = rightSlot;
        if (flipSlot >= 0) turner.flipPageSlot = flipSlot;

        turner.chloe = Object.FindFirstObjectByType<CharacterPresenter>();
        turner.danceMode = Object.FindFirstObjectByType<DanceModeController>();
        turner.gameMode = Object.FindFirstObjectByType<GameModeController>();

        EditorUtility.SetDirty(turner);
    }

    private static Renderer FindRenderer(GameObject book, string childName)
    {
        Transform child = FindDescendant(book.transform, childName);

        return child != null ? child.GetComponent<Renderer>() : null;
    }

    private static string DescribeSlots(Material[] materials)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < materials.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append('[').Append(i).Append("] ").Append(materials[i] != null ? materials[i].name : "(none)");
        }

        return sb.ToString();
    }

    /// <summary>
    /// One log entry describing every renderer under the book. The UV bounds column
    /// is the one that decides things: a face meant to carry a full-page image has
    /// UVs spanning roughly 0..1 in both axes, while a wedge of page edges does not.
    /// </summary>
    private static void Report(GameObject book)
    {
        Renderer[] renderers = book.GetComponentsInChildren<Renderer>(true);

        StringBuilder sb = new StringBuilder();
        // Deliberately not "a page face shows 0..1": it was written that way, and it
        // was wrong. On this model the page EDGE spans the square too, so the span
        // says whether a submesh can carry a full image, never whether it is the one
        // you read. The material name decides that; this measures.
        sb.Append("[BookPages] '").Append(book.name).Append("' has ").Append(renderers.Length)
          .Append(" renderer(s). UV span says whether a submesh CAN carry a full image, not whether\n")
          .Append("it is the reading face — an edge strip can span the square just as well:\n");

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            sb.Append("  ").Append(r.name).Append("\n");

            MeshFilter filter = r.GetComponent<MeshFilter>();
            Mesh mesh = filter != null ? filter.sharedMesh : null;

            if (mesh == null)
            {
                SkinnedMeshRenderer skinned = r as SkinnedMeshRenderer;
                mesh = skinned != null ? skinned.sharedMesh : null;
            }

            if (mesh == null)
            {
                sb.Append("      mesh: none\n");
                continue;
            }

            sb.Append("      mesh: ").Append(mesh.name)
              .Append("  submeshes: ").Append(mesh.subMeshCount)
              .Append("  verts: ").Append(mesh.vertexCount).Append("\n");

            // World bounds, because a part can be painted perfectly and still be
            // invisible with another part lying on top of it — which is what made the
            // right page read as blank while its texture was on it the whole time.
            sb.Append("      bounds: centre ").Append(r.bounds.center.ToString("F3"))
              .Append("  size ").Append(r.bounds.size.ToString("F3")).Append("\n");

            Renderer sourceRenderer = PrefabUtility.GetCorrespondingObjectFromSource(r) as Renderer;
            Material[] original = sourceRenderer != null ? sourceRenderer.sharedMaterials : null;
            Material[] mats = r.sharedMaterials;

            // Per SUBMESH, not per mesh — and that distinction is the whole lesson
            // here. Measured whole-mesh, PageBlock_L reported a full 0-1 span and was
            // labelled a page face, which was true of the paper submesh and false of
            // the bound edge sharing the same mesh. Every slot looked equally good and
            // the first guess went to the wrong one.
            for (int s = 0; s < mesh.subMeshCount; s++)
            {
                sb.Append("      [").Append(s).Append("] ")
                  .Append(s < mats.Length && mats[s] != null ? mats[s].name : "(no material)");

                if (original != null && s < original.Length && original[s] != null && original[s] != mats[s])
                    sb.Append("  (model: ").Append(original[s].name).Append(")");

                sb.Append("\n          uv: ").Append(DescribeSubmeshUv(mesh, s)).Append("\n");
            }
        }

        Debug.Log(sb.ToString(), book);
    }

    private static string DescribeSubmeshUv(Mesh mesh, int submesh)
    {
        Vector2[] uv = mesh.uv;

        if (uv == null || uv.Length == 0) return "NONE — this mesh cannot show a texture correctly";

        int[] indices = mesh.GetTriangles(submesh);

        if (indices == null || indices.Length == 0) return "no triangles";

        float minU = float.MaxValue, maxU = float.MinValue;
        float minV = float.MaxValue, maxV = float.MinValue;

        for (int i = 0; i < indices.Length; i++)
        {
            int v = indices[i];
            if (v < 0 || v >= uv.Length) continue;

            if (uv[v].x < minU) minU = uv[v].x;
            if (uv[v].x > maxU) maxU = uv[v].x;
            if (uv[v].y < minV) minV = uv[v].y;
            if (uv[v].y > maxV) maxV = uv[v].y;
        }

        string span = string.Format("U {0:F3}..{1:F3}  V {2:F3}..{3:F3}", minU, maxU, minV, maxV);

        if (maxU > 1.001f || minU < -0.001f || maxV > 1.001f || minV < -0.001f)
            return span + "   <- spills outside 0-1, the texture will tile/repeat here";

        bool fillsU = (maxU - minU) > 0.8f;
        bool fillsV = (maxV - minV) > 0.8f;

        if (fillsU && fillsV) return span + "   <- spans the full square";

        return span + "   <- covers only part of the square";
    }
}
