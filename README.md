# Seweriously Flooded

Takım: pixAlien
Etkinlik: Civil Game Jam
Motor: Unity 6000.6.6f1
Platform: Windows

## Oyun Hakkında

Şehirde yağmur başlamak üzere ve lağımlar yapraklarla, kartonlarla tıkanıyor. Yağmur başlamadan önce hazırlanın, sonra lağımları açık tutmak için zamana karşı yarışın. Lağımlar ne kadar tıkalı olursa su o kadar hızlı yükselir. Su belli bir seviyeye ulaşınca sokakları sel basar ve oyun biter.

Amaç olabildiğince uzun süre hayatta kalmak ve olabildiğince çok yaprak toplayarak sel felaketini önlemek. Skor, hayatta kalma süresi ile toplanan yaprak sayısından hesaplanır; en iyi skor kaydedilir.

## Kurulum ve Çalıştırma

1. ZIP dosyasını bir klasöre çıkarın.
2. `Pixalien_GameJam.exe` dosyasını çalıştırın. 
3. Ek bir kurulum gerekmez.

Oyun 16:9 için tasarlanmıştır, farklı ekran oranlarında da tüm oyun alanı görünür.

### Projeyi Unity'de Açmak

1. Bu repoyu klonlayın.
2. Unity Hub ile projeyi açın (Unity 6, yukarıdaki sürüm).
3. `Assets/Scenes/SampleScene` sahnesini açıp Play'e basın.

## Kontroller

| Tuş | İşlev |
|---|---|
| Sağ tık (RMB) : Tıklanan yere koş. Lağımın yanına gidince lağım temizlenir. |
| Sol tık (LMB), art arda : Lağımı tıkayan kartonu almak için spamlayın. |
| ESC : Oyunu duraklat / devam et. Tutorial ekranını kapat. |

## Nasıl Oynanır

- Lağımlara koşun ve temizleyin. Toplanan yapraklar çantanıza girer.
- Çantanıza dikkat edin: daha çok yaprak, daha çok ağırlık! Ağırlaştıkça yavaşlarsınız.
- Kartonları almak için sol tıka art arda basın.
- Yaprakları ve kartonları çöp konteynerlerine atın.
- Selin gelmesini engelleyin!

## Kullanılan Hazır Kaynaklar

| Kategori | Kullanıldı mı | Açıklama |
|---|---|---|
| Görsel | evet | Neredeyse tüm görseller pixAlien ekibi tarafından jam sırasında çizildi (AI ile üretilenler aşağıda sadece 3 tane). |
| Ses | Evet | Yürüme, yağmur, trafik uğultusu ve temizleme sesleri hazır kaynaklardan alındı. Kaynaklar ve lisanslar aşağıda. 
| Kod | Evet | Tüm scriptler Claude ve Gemini ile yazıldı. |
| Metin | Hayır | |
| Diğer | Evet | Yazı tipi: 8-bit-pusab. |

**Ses ve yazı tipi kaynakları:**
- Yürüme sesi:https://freesound.org/people/InspectorJ/sounds/329607/, opensource
-su sesi : https://freesound.org/people/SignatureSoundsOrg/sounds/873027/, opensource (kullanılmadı)
-su damlama sesi : https://freesound.org/people/SignatureSoundsOrg/sounds/873026/, opensource (kullanılmadı)
-su sesi 2 : https://freesound.org/people/SignatureSoundsOrg/sounds/873024/, opensource (kullanılmadı)
- Yağmur sesi:[ _kaynak bağlantısı_](https://freesound.org/people/Ow.ethan.james/sounds/765793/), opensource
- Trafik uğultusu: https://freesound.org/people/angelkunev/sounds/736433/,opensource
- Temizleme sesi: [_kaynak bağlantısı_](https://freesound.org/people/Breviceps/sounds/444494/),opensource
- 8-bit-pusab yazı tipi:https://www.dafont.com/8-bit-pusab.font, opensource

## Yapay Zeka Kullanımı

| Kategori | Kullanıldı mı | Araç | Amaç ve kullanım yeri |
|---|---|---|---|
| Görsel |*Evet | Claude (Anthropic) | Yağmur damlası yere düşünce çıkan 4 kareli pixel halka animasyonu (`RainRipple`), lağımlardan çıkan kabarcıkların patlama animasyonu (`BubblePop`) ve tutorial yazı görselleri, "CLICK RMB TO WALK" yazısı
| Ses | Hayır | | 
| Kod | Evet | Gemini (Google), Claude (Anthropic) | Oyun scriptleri yapay zeka yardımıyla yazıldı; ekip tarafından düzenlenip oyuna entegre edildi. Ayrıntılar aşağıda. |
| Metin | Hayır | 
| Diğer | Evet | Claude (Anthropic) | Unity kurulumu, UI yerleşimi, ekran çözünürlüğü ve Git kullanımı konularında teknik yardım. |

Kod tarafında AI kullanım yerleri:
- Gemini ile yazılanlar: `PlayerMovement`, `RainManager`, `DrainageNode`, `GlowOnHover` ,`CameraFollow` , `Dumpster`, `Gamemenu`, `PlayerVisuals`,`ScoreManager`,`SpritePulse`,`VehicleLoop`
- **Claude ile yazılanlar:** `GameMenu` (menüler, pause, game over, tutorial), `ScoreManager` (skor ve en iyi skor), `ButtonHoverScale`, `PanelPopIn` (UI animasyonları), `WaterScroll`, `PixelWater.shader` (su görünümü ve köpük), `WaterTrail`, `PixelParticles` (partikül efektleri), `UIRain`, `UISpriteAnimator` (giriş ekranı), `BoxClickHint`, `HeadHint` (oyun içi ipuçları), `AspectRatioLock` (ekran oranı uyumu)
- `DrainageNode` ve `RainManager` scriptlerinde skor ve oyun sonu bağlantısı için Claude yardımıyla küçük değişiklikler yapıldı. İsmi iki listede de bulunan scriptler iki ai ile tekrar tekrarmodifiye edildi.

## Ekip

**pixAlien**
İsim - rol

Eray Güler , Developer 
Ziya Emre Özçetin , Developer
Selay Yılmaz , Artist ve designer
Can Sadaghiani , sfx ve designer
