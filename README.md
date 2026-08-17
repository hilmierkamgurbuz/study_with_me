# Study With Me

**Seninle birlikte ders çalışan, sesli konuşan bir 3D oda arkadaşı.**

[![Unity](https://img.shields.io/badge/Unity-6000.0.80f1-000000?logo=unity)](https://unity.com/releases/editor/whats-new/6000.0.80)
[![Render Pipeline](https://img.shields.io/badge/URP-17.0.4-2196F3)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/index.html)
[![Gemini Live](https://img.shields.io/badge/Gemini-Live%20API-4285F4?logo=google)](https://ai.google.dev/gemini-api/docs/live)
[![Platform](https://img.shields.io/badge/platform-Editor%20%7C%20Standalone-lightgrey)]()

![Chloe masasında](docs/screenshots/01-desk.jpg)

---

## English summary

**Study With Me** is a Unity study-companion app. A 3D character named Chloe sits at her desk in her room and talks to you **by voice, in real time**, through the **Gemini Live API** — real speech in, real speech out, with live captions and the ability to interrupt her mid-sentence.

She remembers you between sessions. During the conversation she saves what she learns about you — your name, what exam you're studying for, how long you usually work — and the next time you open the app she greets you by name and picks up where you left off.

She also lives in the room rather than waiting in a menu. Between conversations she turns to the book on her desk and reads it, page by page. On demand the lights drop and the room turns into a disco, cat and dog included. On demand again the camera moves to the TV and hands the screen over to a fruit-merge minigame, while the voice connection stays alive behind it.

**The rest of this README is in Turkish.**

---

## Ne yapıyor?

Study With Me, ders çalışırken yalnız hissetmemek için yapılmış bir uygulama. Ekranda odasında oturan **Chloe** var; onunla mikrofonla, gerçek zamanlı olarak Türkçe konuşuyorsun. Kazanma/kaybetme yok — bu bir oyun değil, bir çalışma arkadaşı.

Uygulamayı açıyorsun ve Chloe seni karşılıyor. İlk defa tanışıyorsanız sohbetin doğal akışında adını, sınıfını, hangi sınava hazırlandığını, ne kadar çalıştığını öğreniyor — anket doldurtur gibi değil, konuşurken. Öğrendiklerini kendisi kaydediyor, bir dahaki açışında seni ismiyle karşılayıp geçmişe gönderme yapıyor.

Sen çalışırken o boş boş durmuyor. Kitabını okuyor, sayfa çeviriyor. Canın sıkılırsa odayı diskoya çeviriyor. Mola vermek istersen ekranı bir mini oyuna devrediyor.

---

## Ekran görüntüleri

### Konuşma

![Konuşma](docs/screenshots/01-desk.jpg)

Masasında oturuyor ve seni dinliyor. Yüz ifadesi konuşmaya göre değişiyor — konuşurken ağzı hareket ediyor, dinlerken ve düşünürken farklı ifadeler takınıyor.

### Çalışma modu

![Çalışma modu](docs/screenshots/02-study-mode.jpg)

Konuşma yokken kitabına dönüp okuyor. Sayfa çevirirken eli gerçekten kitaba uzanıyor.

### Oyun modu

![Oyun modu](docs/screenshots/03-game-mode.jpg)

Kanepeye geçiyor, kamera TV'ye yaklaşıyor ve ekran mini oyuna geçiyor. Oda arka planda yaşamaya devam ediyor, sesli bağlantı kopmuyor. `Esc` ile geri dönülüyor.

### Dans modu

![Dans modu](docs/screenshots/04-dance-mode.jpg)

Işıklar sönüyor, disko topu ve renkli spotlar devreye giriyor, müzik başlıyor. Kedi ve köpek de rotalarını bırakıp partiye katılıyor. İki dakika sonra oda eski haline dönüyor.

---

## Özellikler

### 🎙️ Gerçek zamanlı sesli konuşma

Gemini Live API üzerinden doğrudan ses alışverişi — yazıya çevirip geri okutma yok. Butona basıp konuşuyorsun, o cevap veriyor. Konuşurken araya girebiliyorsun, sözünü kesebiliyorsun. Söylenenler ekranda altyazı olarak da akıyor.

### 🧠 Seni hatırlıyor

Chloe hakkında öğrendiklerini konuşma sırasında kendisi kaydediyor:

- Adın, sınıfın/bölümün
- Hazırlandığın sınav veya hedef
- Genelde ne zaman ve ne kadar çalıştığın
- Kaç dakikada bir mola sevdiğin
- Toplam kaç oturum ve kaç dakika çalıştığınız

Bu bilgiler bilgisayarında yerel bir dosyada duruyor. Bir sonraki açılışta Chloe bunları biliyor olarak geliyor.

### 🎭 Odada yaşıyor

- **Çalışma modu** — kitabına dönüp okuyor, sayfa çeviriyor
- **Dans modu** — ışıklar, disko, müzik, iki dakikalık dans
- **Oyun modu** — ekranı meyve birleştirme mini oyununa devrediyor
- **Kedi ve köpek** — odada dolaşıyorlar, dans modunda partiye katılıyorlar

Şimdilik bu modlar ekrandaki butonlarla açılıyor. İleride konuşmanın kendisi başlatacak.

### 🍉 İçinde bir mini oyun var

Mola verdiğinde açılan tam bir meyve birleştirme oyunu — kendi menüsü, skoru, boost'ları ve kayıt sistemiyle. Odanın üstüne ek olarak yükleniyor, yani oyundan çıkınca Chloe'yle konuşma kaldığı yerden devam ediyor.

---

## Kurulum

### Gereksinimler

- **Unity 6000.0.80f1** (Unity 6)
- Bir **Gemini API key** — [Google AI Studio](https://aistudio.google.com/apikey)'dan ücretsiz alınıyor
- Mikrofon
- macOS veya Windows

### 1. Klonla ve aç

```bash
git clone https://github.com/hilmierkamgurbuz/study_with_me.git
```

Unity Hub → **Add** → klasörü seç → Unity 6000.0.80f1 ile aç. İlk açılış birkaç dakika sürebilir.

### 2. API key'ini gir

> [!IMPORTANT]
> API key koda gömülmüyor ve repoya girmiyor. Key'i tutan dosya `.gitignore`'da, o yüzden klonladığında yok — kendin oluşturuyorsun.

1. **Project** penceresinde `Assets/Config` klasörüne sağ tıkla
2. **Create → StudyWithMe → Gemini API Config**
3. Dosyaya **`GeminiApiConfig`** adını ver
4. Inspector'da **Api Key** alanına AI Studio'dan aldığın key'i yapıştır
5. `Room.unity` sahnesini aç, `GeminiLiveVoiceSession` bileşenini bul ve **Config** alanına bu dosyayı sürükle

> [!NOTE]
> Bağlantı kurulmuyorsa model adı eskimiş olabilir — Live API modelleri preview seviyesinde ve dönüyor. [Güncel listeye](https://ai.google.dev/gemini-api/docs/models) bakıp **Live Model Id** alanını güncelle.

### 3. Mini oyunun ayarlarını uygula

Menüden **Tools → Fruit Merge → Apply Import Settings** komutunu bir kez çalıştır. Bu, mini oyunun ihtiyaç duyduğu katman adlarını ve sahne kaydını ayarlıyor.

### 4. Çalıştır

`Assets/Scenes/Room.unity` sahnesini aç ve Play'e bas.

---

## Kullanım

| Ne yapmak istiyorsun | Nasıl |
|---|---|
| Chloe'ye bağlan | Sol üstteki **"Chloe'ye bağlan"** butonu |
| Konuş | **"Konuşmak için tıkla"** — bir kez tıkla konuş, tekrar tıkla bırak |
| Sözünü kes | O konuşurken tıkla |
| Ders çalışmasını izle | **"Ders Çalış"** butonu |
| Dans ettir | **"Dance Mode"** butonu |
| Mini oyunu aç | **"Oyun Oyna"** butonu — çıkış `Esc` |

Sol üstteki bağlantı arayüzü geçici bir test arayüzü; gerçek arayüz henüz yazılmadı.

---

## Proje durumu

Bu bir **dikey dilim** — uçtan uca çalışan gerçek bir sürüm, ama ürünün tamamı değil.

**Çalışıyor:** sesli konuşma, araya girme, altyazılar, profil hafızası ve oturumlar arası süreklilik, yüz ifadeleri ve animasyon, çalışma/dans/oyun modları, kedi ve köpek, mini oyun entegrasyonu.

**Henüz yok:** oturum akışının otomatikleşmesi (mola teklifi, oturum başlangıç/bitiş adımları), gerçek arayüz katmanı, oturum sonu özeti, modların konuşmayla açılması.

**İleride:** Android + iOS + WebGL, cihaz diline göre çok dillilik, abonelik modeli, final karakter sanatı.

---

## Teknik altyapı

| | |
|---|---|
| Motor | Unity 6 (6000.0.80f1), URP |
| Ses | [Gemini Live API](https://ai.google.dev/gemini-api/docs/live) — native ses, function calling |
| Bağlantı | [NativeWebSocket](https://github.com/endel/NativeWebSocket) |
| JSON | Newtonsoft.Json for Unity |
| Giriş | Unity Input System |
| Kayıt | Yerel JSON dosyası |

Kodun tamamı `Assets/Scripts/` altında; sistemler `Voice`, `Persistence`, `Presentation`, `Config` ve `Bootstrap` olarak ayrılmış. Mini oyun `Assets/FruitMerge/` altında kendi içinde kapalı duruyor.

Mimari kararlar ve gerekçeleri `.claude/` klasöründe belgeleniyor.

---

## Krediler

**Sanat ve ses:** Oda ve karakter varlıkları Unity Asset Store'daki ücretsiz paketlerden — FreeStylizedBedRoom, FREE_STYLIZED_GAMEROOM_PACK, PolyOne Cartoon Dog & Cat. Chloe'nin modeli bu paketlerden birinin karakterinin Blender'da yeniden düzenlenmiş hali. Animasyonlar Mixamo'dan, dans müziği lisanslı bir müzik paketinden.

Üçüncü taraf paketler kendi lisanslarına tabidir.

**Kod:** `Assets/Scripts/`, `Assets/Editor/` ve `.claude/` altındaki her şey bu projeye ait.
