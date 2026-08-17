# Postflight: Chloe kol-maskesi + katmanlı Animator kurulumu

(Görev seyir sırasında revize edildi: maske sınırı "bacaklar hariç"ten "sadece kollar"a daraltıldı — kullanıcı yönlendirmesi. D-018 kurulumu, D-019 daraltmayı kaydediyor.)

- [Y] No procedure violations — `ownership.md`: kol zincirlerinin tek yazarı maskeli `Arms` katmanı, geri kalan 34 transform'un tek yazarı Base Layer; maske ikisini kesişmez kılıyor. `data-source.md`: sınır koda değil `.mask` varlığına yazıldı.
- [Y] Located, not scanned — locate step 1 (`index.md` → Presentation/Chloe) + step 3 (`codemap-editor.md`); tüm okuma `Assets/Art/chloe/` ve `Assets/Scripts/Presentation/` ile sınırlı kaldı. Alt-ajan kullanılmadı.
- [Y] Invariant scan clean — C# yazılmadı; içerik koda gömülmedi (46 transform yolu `ModelImporter.transformPaths`'ten okundu); `CharacterPresenter`, `Chloe.prefab`, `Room.unity` hiç değiştirilmedi.
- [Y] Proof standard met (production) — riskli varsayım ölçüldü, tahmin edilmedi: iki klip tek kullanımlık bir model örneği üzerinde 44.97 s boyunca 7 noktada örneklendi, maskeli sonuç elle kompoze edildi; el-kafa mesafesi yazıldığı hâlden en fazla 1.1 cm (sağ) / 2.0 cm (sol) sapıyor. Katman maliyeti: tek karakter, 20 kemik, kare başına O(1) — eşiğin çok altında.
- [Y] Codemap updated — kod dosyası yazılmadı; iki varlık `blueprint.md` `Art/chloe/` satırında güncel adlarıyla duruyor.
- [Y] decisions.md updated — D-018 (kurulum) + D-019 (daraltma, ölçüm sonuçlarıyla), ikisi de `affects:` alanlı.
- [Y] Assumptions closed — "kollar eğik göğse göre yazılmış, taban düzelince eller kayar" varsayımı **ölçülerek çürütüldü** (≤2 cm sapma) ve D-019'a yazıldı. WriteDefaults=0 varsayımı diskten doğrulandı.
- [Y] Editor side synced — diskten geri okundu: `ChloeArmsOnly.mask` 12 açık / 34 kapalı; yeniden adlandırma guid'i (`81bee1b4…`) korudu, controller referansı sağlam; eski `ChloeUpperBody.mask` diskte yok; katmanlar `Base Layer` (masksız) ve `Arms` (maskeli, Override, weight 1); `IsTalking` geçişleri orijinal state machine nesnesinde taşındı.
- [Y] Blueprint consistent — `python3 .claude/hooks/check_blueprint.py` → `0 error(s), 3 warning(s), 2 info.` Üç uyarı da görevden önce vardı.
- [Y] Shipping signals checked — Session ve UI sistemleri hâlâ kodsuz; faz geçişi uzak.

## Kullanıcıya taşınan açık maddeler

1. `speakingPositionOffset` / `speakingRotationOffset` hâlâ sahnede eski değerlerinde. Artık telafi ettikleri eğilme kafaya ulaşmıyor; sıfırlanmaları gerekiyor. `Room.unity` korumalı tip olduğu için ve kadraj kullanıcının kararı olduğu için Claude tarafından yazılmadı.
2. Kamera bilinçli olarak değiştirilmedi (kullanıcı tercihi). Ölçülen durum kayıt için: kamera Y=1.267, kafa Y=1.290, geometrinin istediği pitch −1.2°, gerçek pitch +12.0°.
