using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the app's one piece of UI — the microphone button — and wires it to
/// the session controller. Re-runnable, and a button that has been dragged
/// somewhere is never moved back.
/// </summary>
public static class SessionUiSetup
{
    private const string IdleSpritePath = "Assets/Art/Textures/microphone-button.png";
    private const string ListeningSpritePath = "Assets/Art/Textures/microphone-button-sound.png";

    private const string UiObjectName = "--SessionUI--";
    private const string ButtonObjectName = "TalkButton";

    // Screen Space Overlay already draws after every camera-space canvas, so the
    // vendored game (which tops out at 201) cannot cover this whichever number is
    // here. The order is set anyway, for room canvases added later.
    private const int UiSortingOrder = 500;

    private const float ButtonSize = 160f;
    private const float ButtonMargin = 120f;

    [MenuItem("Tools/StudyWithMe/Set Up Session UI")]
    public static void SetUp()
    {
        Sprite idle = EnsureSprite(IdleSpritePath);
        Sprite listening = EnsureSprite(ListeningSpritePath);

        var controller = Object.FindFirstObjectByType<RoomSessionController>();

        if (controller == null)
        {
            Debug.LogError("[SessionUiSetup] açık sahnede RoomSessionController yok — Room.unity'yi aç.");
            return;
        }

        PushToTalkButtonView view = FindOrCreateButton(idle);

        Undo.RecordObject(view, "Wire talk button");
        view.idleSprite = idle;
        view.listeningSprite = listening;

        // Assigned on EVERY run, not only when the button is created: a button
        // built by an earlier run with a null sprite would otherwise stay a white
        // square forever, since Start() — which is what repaints at runtime —
        // never runs in edit mode. This is only the resting look; Repaint() owns
        // the sprite from the first frame of Play onwards.
        if (idle != null && view.image != null)
        {
            Undo.RecordObject(view.image, "Wire talk button");
            view.image.sprite = idle;
            EditorUtility.SetDirty(view.image);
        }

        Undo.RecordObject(controller, "Wire talk button");
        controller.talkButton = view;

        EditorUtility.SetDirty(view);
        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(controller.gameObject.scene);

        Debug.Log(string.Format(
            "[SessionUiSetup] {0} hazır — boştaki sprite:{1} dinlerken sprite:{2}. " +
            "Sahne kirli işaretlendi, kaydetmedim.",
            UiObjectName, idle != null, listening != null), view);
    }

    /// <summary>
    /// A PNG dropped into the project imports as a plain Texture, and
    /// LoadAssetAtPath&lt;Sprite&gt; then returns null — which reads as "the sprite
    /// was not assigned" while the real cause is an import setting. So the type is
    /// corrected here rather than left as a step to remember.
    ///
    /// The mode matters as much as the type, and that is what actually bit: with
    /// textureType already Sprite but spriteImportMode left on Multiple, the main
    /// asset at this path is still the Texture2D — sprites live as sliced
    /// sub-assets, none are defined, and the load silently returns null. Single is
    /// the right answer here: this is one button graphic, not an atlas.
    /// </summary>
    private static Sprite EnsureSprite(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("[SessionUiSetup] görsel bulunamadı: " + path);
            return null;
        }

        if (importer.textureType != TextureImporterType.Sprite ||
            importer.spriteImportMode != SpriteImportMode.Single)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
            Debug.Log("[SessionUiSetup] import ayarı düzeltildi (Sprite / Single): " + path);
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        // Never hand a null onward quietly: a null here is exactly what draws
        // Unity's default white square, which says nothing about why.
        if (sprite == null)
            Debug.LogError("[SessionUiSetup] Sprite olarak yüklenemedi: " + path +
                           " — import ayarlarını elle kontrol et (Texture Type: Sprite, Sprite Mode: Single).");

        return sprite;
    }

    private static PushToTalkButtonView FindOrCreateButton(Sprite idle)
    {
        PushToTalkButtonView existing = Object.FindFirstObjectByType<PushToTalkButtonView>();

        if (existing != null && existing.button != null && existing.image != null) return existing;

        // Half-built, from a run that did not finish. Keeping it would mean a
        // re-run "finds" it forever and the app ends up with no way to talk.
        if (existing != null)
        {
            Debug.LogWarning("[SessionUiSetup] " + UiObjectName + " eksikti; yeniden kuruluyor.", existing);
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        GameObject root = new GameObject(UiObjectName, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(root, "Create " + UiObjectName);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = UiSortingOrder;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        root.AddComponent<GraphicRaycaster>();

        GameObject go = new GameObject(ButtonObjectName, typeof(RectTransform));
        go.transform.SetParent(root.transform, false);

        // Bottom-left: the minigame is held to a centred portrait viewport, so on a
        // wide screen this corner is letterbox rather than board — the button does
        // not cover gameplay and cannot swallow a tap meant for it.
        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(ButtonSize, ButtonSize);
        rect.anchoredPosition = new Vector2(ButtonMargin, ButtonMargin);

        go.AddComponent<CanvasRenderer>();

        Image image = go.AddComponent<Image>();
        image.sprite = idle;
        image.preserveAspect = true;
        // No background graphic behind it: the art carries the button's whole
        // shape, ring included, and that ring is what keeps it readable on both
        // the warm room and the bright game.

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        // The default ColorTint transition is left alone: it works on the
        // CanvasRenderer while the view dims the Graphic, so the two multiply
        // instead of overwriting each other and a press still gives feedback.

        PushToTalkButtonView view = root.AddComponent<PushToTalkButtonView>();
        view.button = button;
        view.image = image;

        return view;
    }
}
