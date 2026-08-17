# Chloe — Yüz İfadesi & Kıyafet Atlas Planı (gerçek ölçülerle, final)

## Doğrulanan teknik gerçekler (tahmin değil, mesh'in gerçek UV verisinden ölçüldü)

- **Yüz**: `Chloe_Face_Alpha.png`'den mesh'in gerçekten örneklediği bölge **621×499 px** — kaş+göz+ağız+yanak allığı TEK PARÇA (birbirinden ayrı UV adaları değil). Yani her ifade bu tam bloğun bir kopyası olacak; kaş/göz/ağız o blok içinde birlikte değişiyor.
- **Vücut/kıyafet**: Chloe'nin kullandığı gerçek parçalar (küçük gürültü adaları hariç):
  - Mavi ceket/kazak (cepsiz): **333×154 px**
  - Kırmızı/bordo pantolon: **~178×181 px**
  - Ten rengi yaka/boyun şekli: **237×153 px**
  - İki ince renk-rampası şeridi (47×607 ve 17×470) — saç/ten tonu gibi bazı renkler muhtemelen buradan geliyor (dolaylı/prosedürel), yeni atlas'ta bu dolaylılığı kullanmayacağız, renkleri direkt boyayacağız.
- **Şapka**: Ayrı, belirgin bir doku adası bulamadım — ekrandaki koyu başlık/saç örtüsü büyük olasılıkla **düz bir renk** (vertex color veya küçük bir rampa örneği), çizilmiş bir desen değil. Bunu doğrulamak için: Unity'de Chloe'ye yakınlaş, başlığında herhangi bir desen/doku görüyor musun, yoksa tamamen düz mü? Düzse, şapka için "çizilmiş texture" yerine sadece **5 farklı düz renk** yeterli olur (aşağıda buna göre planladım, düz renk varsayımıyla).

## Renk hedefleri (referans görselinden)

- **Göz rengi**: kahverengi/amber (mevcut yeşilin yerine)
- **Saç rengi**: sıcak kestane kahve `#6B4226` + karamel vurgu `#A9744F`
- **Ten rengi**: mevcuttan biraz daha açık
- Bu üçü tüm 16 yüz ifadesinde VE tüm kıyafet/saç görünen parçalarında **birebir aynı** kalmalı — sadece ifade/kıyafet değişsin, renkler sabit.

---

## Bölüm 1 — Yüz İfadeleri (4×4 = 16 hücre)

**Hücre boyutu: 640×512 px** (mesh'in 621×499'luk penceresinden güvenli pay ile büyük) → **toplam tuval: 2560×2048 px**.

| # | Konum | Kaş | Göz | Ağız |
|---|---|---|---|---|
| 1 | 1,1 | Rahat/nötr | İleri bakış, kahverengi | Kapalı, nötr |
| 2 | 1,2 | Rahat/nötr | İleri bakış | Hafif aralık — küçük konuşma |
| 3 | 1,3 | Rahat/nötr | İleri bakış | Orta açık — orta konuşma |
| 4 | 1,4 | Rahat/nötr | İleri bakış | Geniş açık — vurgulu konuşma |
| 5 | 2,1 | Rahat/nötr | **Sola bakış** | Kapalı, nötr |
| 6 | 2,2 | Rahat/nötr | **Sağa bakış** | Kapalı, nötr |
| 7 | 2,3 | Rahat/nötr | **Hafif aşağı bakış** | Kapalı, nötr |
| 8 | 2,4 | Rahat/nötr | Gözler kapalı (göz kırpma) | Kapalı, nötr |
| 9 | 3,1 | Yumuşak kalkık | İleri, sıcak | Kapalı gülümseme |
| 10 | 3,2 | Kalkık, mutlu | İleri, mutlu | Açık kahkaha/büyük gülme |
| 11 | 3,3 | Kalkık, şaşkın | İri açık | Küçük yuvarlak "oh" — şaşkınlık |
| 12 | 3,4 | Düz/gevşek | Yarı kapalı göz kapağı | Düz çizgi — sıkılmış/ilgisiz |
| 13 | 4,1 | Gevşek | Kısık | Esneme şeklinde açık — uykulu |
| 14 | 4,2 | Bir kaş kalkık | İleri, meraklı | Hafif yana kaymış — düşünceli |
| 15 | 4,3 | Yumuşak | Sıcak, ilgili | Hafif gülümseme — şefkatli/dinleyen |
| 16 | 4,4 | Hafif kalkık, oyuncu | İleri, parıltılı | Yana kaymış küçük gülümseme — şakacı |

**AI'a verilecek prompt (İngilizce, kopyala-yapıştır):**

```
Create a 2560x2048 texture sheet with a 4x4 grid of 16 equal cells, each cell
exactly 640x512 pixels with no gaps or padding between cells and no visible
grid lines in the final output.

Each cell contains the SAME stylized cartoon female character face — identical
art style, line weight, and camera angle across all 16 cells. Character
details, consistent in every cell:
- warm amber/brown eyes (not green)
- fair/light skin tone
- soft pink cheek blush
- flat game-texture lighting, no baked directional shadows
- front-facing head, centered in each cell with consistent margin

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

No text, no watermark. Cells must align exactly to the 640x512 grid for
UV-offset based sprite-sheet swapping in a game engine.
```

---

## Bölüm 2 — Kıyafetler (ayrı texture, kod ile materyal değişimi)

Her biri **kendi gerçek ölçüsünde**, ayrı bir PNG dosyası olacak. 5 kombin, her kombin 2 parça (ceket + pantolon):

### Ceket/kazak — her biri 333×154 px

```
Create a 333x154 pixel image of a stylized cartoon jacket/sweater, matching
this exact silhouette: a simple crew-neck top with two straight sleeves
extended horizontally (T-pose flat garment shape, no pockets), as shown in
the reference image [Chloe_Body_Island_11.png referans olarak ekle]. Flat
game-texture lighting, no baked shadows, transparent background outside the
garment shape, no text or watermark.

Variant 1: dusty lavender color, soft cozy knit texture
Variant 2: cream color, chunky knit texture with a subtle cable-knit pattern
Variant 3: pastel sage-green color, smooth soft cotton texture
Variant 4: soft pink color with a tiny white star/dot pattern
Variant 5: mustard-yellow color, soft cardigan-like texture
```

### Pantolon — her biri 178×181 px

```
Create a 178x181 pixel image of stylized cartoon pants, matching this exact
silhouette: front-view pants with two rounded pocket flaps at the top corners,
as shown in the reference image [Chloe_Body_Island_0.png veya Island_15.png
referans olarak ekle]. Flat game-texture lighting, no baked shadows,
transparent background outside the garment shape, no text or watermark.

Variant 1: black color, smooth leggings texture
Variant 2: plaid pattern in cream and brown tones, soft pajama-pants texture
Variant 3: denim blue color, casual shorts/joggers texture
Variant 4: soft light-blue color with a tiny white star/dot pattern (matching the jacket in Variant 4)
Variant 5: cream color, soft leggings texture
```

### Yaka/boyun bölgesi — 237×153 px (tüm kombinlerde muhtemelen sabit kalabilir, değişmeyebilir)

```
Create a 237x153 pixel image of a rounded neckline/collar shape, skin-toned,
matching this exact silhouette: [Chloe_Body_Island_4.png referans olarak
ekle]. Fair/light skin tone (matching the face reference), flat lighting, no
shadows, transparent background outside the shape, no text or watermark.
```

### Şapka — belirsizlik var (yukarıdaki not), varsayımsal düz renk yaklaşımı

Eğer düz renkse (desensizse), 5 varyant için sadece şu renkleri kullan (AI'a görsel ürettirmeye gerek yok, direkt bu hex kodlarını materyale uygula):
1. Gri-lila `#A79BB5`
2. Krem-kahverengi `#C9B79C`
3. Adaçayı yeşili `#9CAF88`
4. Yumuşak pembe `#F2B8C6`
5. Hardal `#C9A227`

Eğer gerçekten çizilmiş bir texture'sı varsa (deseni/detayı görürsen), bana söyle — o zaman bu bölüm için de boyutlu bir AI promptu hazırlarım.

---

## Bölüm 3 — Saç ve Ten (taban texture, AI gerekmez)

Doğrulandı: Body mesh'te vertex color YOK (`colors32` boş) — yani renk `material.color` ile de, vertex color ile de kod tarafından ayarlanamıyor. Şu an paylaşılan atlas'ta bu iki renk, iki ince gradyan-rampası şeridinden UV ile örekleniyor (dolaylı, kırılgan bir mekanizma). Chloe artık kendi bağımsız texture'larına geçtiği için bu rampaya hiç ihtiyacımız yok:

- Chloe'nin yeni taban texture'ında (skin/hair UV bölgesi), rampa yerine hedef renkleri **direkt düz/basit dolgu olarak boya**:
  - Ten: mevcuttan biraz açık ten tonu (referans görseldeki gibi)
  - Saç: `#6B4226` taban + `#A9744F` karamel vurgu (ince highlight şeritleri, düz stilize low-poly çizim tarzında — AI'a ayrıca bir prompt yazmaya gerek yok, bu küçük bir düz/basit dolgu, elle de boyanabilir)
- Bu taban texture **hiçbir kıyafet varyantından etkilenmeyecek** — 5 kombin arasında geçiş yaparken sadece ceket/pantolon texture'ı değişecek, saç/ten sabit kalacak.

---

## Kod tarafında kullanım (bilgi amaçlı)

- **Yüz**: `material.SetTextureOffset("_MainTex", new Vector2(col * 0.25f, row * 0.25f))` — 4x4 grid, hücre başı 0.25 UV birimi (640/2560 = 0.25, 512/2048 = 0.25 — tam oturuyor).
- **Kıyafet**: her parça (ceket, pantolon) kendi materyaline/gerekirse compositing'e göre bağımsız `Texture2D` ataması ile değiştirilecek — bunu birlikte kodlarken netleştiririz.
