# Postflight: Dans — Aşama 1 (klip üretimi + Animator dans state'leri)

- [Y] No procedure violations — `ownership.md`: `Generated/` klasörünün tek yazarı hâlâ `ChloeClipPathFixer`, elle `.anim` yazılmadı. `data-source.md`: klip içeriği varlıkta, hangi klibin çalacağı Animator parametresinde — koda gömülü hiçbir şey yok. `recompute-timing.md`: üretim editör zamanı, çalışma anı maliyeti sıfır. `abstraction-level.md`: yeni sınıf/arayüz yok.
- [Y] Located, not scanned — locate step 1 (`index.md` → Presentation + Tooling) ve step 3 (`codemap-editor.md`, aracın sözleşmesi); okuma `Assets/Art/chloe/` ile sınırlı kaldı, alt-ajan kullanılmadı.
- [Y] Invariant scan clean — bu aşamada C# yazılmadı. İçerik koda gömülmedi. `CharacterPresenter`, maske, prefab, sahne değişmedi; Voice/Session bağımlılık okları etkilenmedi.
- [Y] Proof standard met (production) — üretim öncesi ölçüldü, varsayılmadı: klip başına 80/80 eğri tek anlamlı çözülüyor (çözülemeyen 0), 7 kök eğrisi düşüyor. Çalışma anı maliyeti değişmedi (aynı Animator, iki katman, klip sayısı arttı ama aynı anda bir state aktif).
- [Y] Codemap updated — gerekmedi: `.cs` dosyası yazılmadı, mevcut satırların içeriği değişmedi.
- [Y] decisions.md updated — D-022, ölçümler ve `affects:` alanıyla; iki-katmana-kopyalama kararının gerekçesi ve riski dahil.
- [Y] Assumptions closed — (1) `mixamo.com` isim çakışması **üretimden önce** yakalandı ve kaynakta düzeltildi; üç ayrı dosya üretildiği doğrulandı. (2) Kök eğrilerinin düştüğü doğrulandı (kalan kök eğrisi 0), dans yerinde. (3) Katman faz-kilidi varsayımı **hâlâ açık** — ekranda doğrulanacak, bozulursa çare Aşama 2'de `Arms` ağırlığını 0'a çekmek.
- [Y] Editor side synced — controller diskten geri okundu: `IsTalking` akışı bozulmamış (`Sitting Idle ↔ Talking`, 0.5/0.75 sn), altı yeni dans state'i (katman başına üç), AnyState girişleri `IsDancing && DanceClip==n` koşullu, çıkışlar `!IsDancing`, tüm state'lerde WriteDefaults kapalı. Sahne/prefab değişmedi.
- [N] Blueprint consistent — `python3 .claude/hooks/check_blueprint.py` → `0 error(s), 4 warning(s), 2 info.` Hata yok ama uyarı sayısı 3'ten 4'e çıktı.
- [Y] Shipping signals checked — Session ve UI hâlâ kodsuz; faz geçişi uzak.

## NO maddesi: blueprint uyarısı

Dördüncü uyarı `folder on disk but not in the blueprint layout: Assets/Trip Hop Music/`. **Bu görev getirmedi** — kullanıcı müzik paketini import ettiği anda ortaya çıktı. Blueprint'in "Folder layout" bölümüne tanımlayıcı satır eklendi, ama `maps.py`'nin klasör regex'i `^\s*([\w./<>-]+/)` boşluk kabul etmediği için adında boşluk olan klasörler tanımlanamıyor; satır insan için doğru, parser için görünmez.

İki çözüm, ikisi de bu görevin manifestinin dışında olduğu için uygulanmadı:
1. `check_blueprint.py`'deki `IGNORED_TOP` kümesine `"Trip Hop Music"` eklemek — `"Art"` ve `"TextMesh Pro"` zaten orada, yani içtihada uygun. D-006/D-009 ile aynı geri-dönme riski (`init_project.py` yeniden çalışırsa silinir).
2. `maps.py`'nin regex'ini boşluğa izin verecek şekilde genişletmek — daha genel çözüm, aynı geri-dönme riski.

## Kullanıcıya taşınan açık maddeler

1. Aşama 1'in **beklenen ve doğru** görüntüsü: Chloe masanın başında ayağa kalkıp dans ediyor, sandalyenin içinden geçiyor. `lockPose` onu hâlâ oturma konumuna çiviliyor ve onu dans zeminine taşıyan bir şey henüz yok — Aşama 2'nin işi.
2. Katman faz-kilidi ekranda kontrol edilecek.
3. Aşama 2 kapsamı ve üç açık soru (hangi ışıklar sönecek, Chloe zemine nasıl gidecek, dans nasıl bitecek) hâlâ cevapsız.
