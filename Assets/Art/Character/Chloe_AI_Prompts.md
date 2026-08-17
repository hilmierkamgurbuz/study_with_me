# Chloe — AI Image Generation Prompts (Final, corrected for the independent-material rebuild)

Corrected after the Blender rebuild: Chloe's model (`Assets/Art/Character/deneme.fbx`)
now has 7 fully independent materials (`Chloe_Faces_Alpha`, `Gameroom` [skin],
`Hair`, `Hat`, `Panth` [pants], `Shirt` [jacket], `Shirt Interior`), each with
its own real 3D geometry. This means most color variants need **no AI image at
all** — just a Base Color tint set directly on the material. Only the face
(expression content) and two multi-color patterns (plaid, star/dot) actually
need generated art. This file replaces the earlier version, which incorrectly
carried over silhouette-matching and per-variant image requirements from the
old shared-atlas setup.

---

## 1. Face Expression Sheet — the one priority AI image

**Attach with this prompt:**
- `Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/TEXTURES/Chloe_Face_Alpha.png` (the
  real source texture — top-left region already has 4 existing expression rows
  drawn by the pack's own artist: neutral, eyes-closed, mouth-open/surprised,
  angry. This IS the correct flat-sticker style and scope: only eyebrows, eyes,
  mouth, cheek blush — no hair, ears, neck. Use this as the style/scope/layout
  reference, not any AI-generated attempt.)
- Your own character look-reference image (COLOR/style target only — brown
  hair, brown eyes, light skin; ignore its head shape/angle)

**Canvas: 2560×2048 px — 4×4 grid, each cell exactly 640×512 px, no gaps, no grid lines in output.**

```
Create a 2560x2048 texture sheet with a 4x4 grid of 16 equal cells, each cell
exactly 640x512 pixels with no gaps or padding between cells and no visible
grid lines in the final output.

IMPORTANT: this is a flat 2D game texture asset, NOT a character portrait.
It is a small decal of ONLY facial features (eyebrows, eyes, mouth, cheek
blush) that gets overlaid onto a separate 3D head mesh which already has its
own hair, ears, neck, and skin rendered elsewhere. Do NOT draw hair, ears,
neck, shoulders, or clothing. Do NOT use painterly, semi-realistic, or
soft-shaded rendering. Match the flat vector sticker style of the reference
image exactly: pure flat color fills, hard clean edges, no gradients except
the existing soft blush glow, no outlines around the face shape itself.

Background: pure white (or transparent), NOT a solid color portrait
background.

Every cell must use the EXACT SAME camera framing, zoom, and position of the
eyebrows/eyes/mouth — only the expression itself changes, nothing else moves
or rescales, so switching cells doesn't shift the face.

Character details, consistent in every cell:
- warm amber/brown eyes (not green)
- fair/light skin tone
- soft pink cheek blush (soft glow, like the reference — not a hard circle)
- front-facing, centered, identical scale in every cell

Only the expression (eyebrows + eyes + mouth) changes per cell, as follows
(row, column):
(1,1) relaxed eyebrows, eyes looking forward, mouth fully closed neutral
(1,2) relaxed eyebrows, eyes forward, mouth slightly parted — small talking shape
(1,3) relaxed eyebrows, eyes forward, mouth medium open — mid talking shape
(1,4) relaxed eyebrows, eyes forward, mouth wide open — emphatic talking shape
(2,1) relaxed eyebrows, eyes looking LEFT, mouth closed neutral
(2,2) relaxed eyebrows, eyes looking RIGHT, mouth closed neutral
(2,3) relaxed eyebrows, eyes looking slightly DOWN, mouth closed neutral
(2,4) relaxed eyebrows, eyes fully closed (blinking), mouth closed neutral
(3,1) soft raised eyebrows, warm forward gaze, closed-mouth gentle smile
(3,2) raised happy eyebrows, joyful forward gaze, wide open laughing smile
(3,3) surprised raised eyebrows, wide open eyes, small round "oh" mouth
(3,4) flat/loose eyebrows, half-lidded bored eyes, flat unimpressed mouth line
(4,1) relaxed low eyebrows, squinted sleepy eyes, mouth open in a yawn shape
(4,2) one eyebrow raised, curious forward gaze, mouth slightly to one side — thoughtful
(4,3) soft eyebrows, warm attentive gaze, gentle small smile — caring/listening
(4,4) playfully raised eyebrows, sparkling forward gaze, small smile tilted to one side — cheeky

No text, no watermark, no hair, no ears, no neck, no shoulders, no clothing.
Cells must align exactly to the 640x512 grid for UV-offset based sprite-sheet
swapping in a game engine.
```

**Code-side note:** `material.SetTextureOffset("_MainTex", new Vector2(col * 0.25f, row * 0.25f))` — 4×4 grid, each cell is exactly 0.25 UV units (640/2560 = 512/2048 = 0.25). Applies to `Chloe_Faces_Alpha.mat`.

---

## 2. Flat color tints — NO AI image, just set Base Color in Unity

Each material below is fully independent now, so a plain Base Color change is
enough. In code: `renderer.materials[i].color = myColor;` (or tint per-material
directly in the Inspector for the default look).

| Material | Use | Color(s) |
|---|---|---|
| `Hair.mat` | fixed, all outfits | `#6B4226` (chestnut brown) |
| `Gameroom.mat` (skin) | fixed, all outfits | fair/light skin tone (lighter than current) |
| `Shirt Interior.mat` | fixed, all outfits | one neutral color, e.g. soft white/cream |
| `Hat.mat` | 5 outfit variants | `#A79BB5`, `#C9B79C`, `#9CAF88`, `#F2B8C6`, `#C9A227` |
| `Shirt.mat` | jacket variants 1, 2, 3, 5 | dusty lavender, cream, pastel sage-green, mustard-yellow |
| `Panth.mat` | pants variants 1, 3, 5 | black, denim blue, cream |

`Gameroom.mat` here is the new independent one under `Assets/Art/Character/`
(after you move the `deneme material/` folder there) — NOT the shared vendor
one in `Assets/ZNS3D/...` — confirmed these are separate material assets, so
tinting this one is safe and doesn't touch the other 7 pack characters.

---

## 3. Pattern swatches — the only remaining AI images (2 total)

These are the only two variants that combine more than one color, so a plain
tint can't produce them. Since the mesh now has real 3D geometry (shape comes
from the mesh, not from matching an old flat silhouette), these don't need to
match any old UV crop — generate them as small **seamless/tileable** fabric
swatches; Unity will wrap them around the garment's own UV via the material,
tiling/scaling as needed. Exact pixel size isn't critical — 512×512 is a safe,
simple default.

### Star/dot pattern — jacket Variant 4 + pants Variant 4

```
Create a 512x512 pixel seamless, tileable fabric pattern swatch: small white
4-pointed stars and tiny dots scattered evenly across a solid color
background. Flat-shaded style, no gradients, no shading, no text or
watermark. The pattern must tile seamlessly with no visible seam when
repeated edge-to-edge.

Generate two color versions:
Version A: background color soft pink (#F2B8C6-ish) — for the jacket
Version B: background color soft light-blue — for the pants (matching set)
```

Assign Version A to `Shirt.mat` (jacket Variant 4), Version B to `Panth.mat`
(pants Variant 4).

### Plaid pattern — pants Variant 2

```
Create a 512x512 pixel seamless, tileable plaid/tartan fabric pattern swatch
in cream and brown tones, soft pajama-pants style. Flat-shaded, no gradients,
no shading, no text or watermark. The pattern must tile seamlessly with no
visible seam when repeated edge-to-edge.
```

Assign to `Panth.mat` (pants Variant 2).

---

## Summary — what actually needs AI generation

1. Face expression grid (Section 1) — 1 image, 16 cells.
2. Star/dot swatch — 2 color versions (Section 3).
3. Plaid swatch — 1 image (Section 3).

Everything else (Hair, Skin, Shirt Interior, Hat ×5, Shirt ×3, Panth ×3) is a
plain material Base Color change — zero images.
