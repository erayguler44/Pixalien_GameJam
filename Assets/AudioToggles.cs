using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Müzik ve efekt seslerini ayrı ayrı açıp kapatır.
// "Music Sources" listesindeki AudioSource'lar müzik sayılır,
// sahnedeki diğer TÜM AudioSource'lar SFX sayılır (sonradan oluşanlar dahil).
// Seçim kaydedilir: sahne yeniden yüklense ya da oyun kapatılıp açılsa da hatırlanır.
public class AudioToggles : MonoBehaviour
{
    const string MusicKey = "MusicOn";
    const string SfxKey = "SfxOn";

    public static bool MusicOn { get; private set; } = true;
    public static bool SfxOn { get; private set; } = true;

    [Tooltip("Müzik çalan AudioSource'lar (menü, oyun, game over müzikleri)")]
    [SerializeField] AudioSource[] musicSources;

    [Tooltip("Açıksa, obje adında bu kelime geçen her AudioSource da müzik sayılır")]
    [SerializeField] bool detectMusicByName = true;
    [SerializeField] string musicNameKeyword = "Music";

    [Header("Kontrol (sadece okumak için)")]
    [Tooltip("Oyun sırasında müzik olarak tanınan sesler")]
    [SerializeField] List<string> debugMusicFound = new List<string>();

    [Header("Buton yazıları (opsiyonel, birden fazla menü için)")]
    [SerializeField] TMP_Text[] musicLabels;
    [SerializeField] TMP_Text[] sfxLabels;
    [SerializeField] string musicOnText = "MUSIC: ON";
    [SerializeField] string musicOffText = "MUSIC: OFF";
    [SerializeField] string sfxOnText = "SFX: ON";
    [SerializeField] string sfxOffText = "SFX: OFF";

    [Header("Buton görselleri (opsiyonel, çizilmiş açık/kapalı ikonlar için)")]
    [SerializeField] Image[] musicIcons;
    [SerializeField] Image[] sfxIcons;
    [SerializeField] Sprite musicOnSprite, musicOffSprite;
    [SerializeField] Sprite sfxOnSprite, sfxOffSprite;

    readonly HashSet<AudioSource> music = new HashSet<AudioSource>();
    float rescanTimer;

    void Awake()
    {
        MusicOn = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        SfxOn = PlayerPrefs.GetInt(SfxKey, 1) == 1;

        if (musicSources != null)
            foreach (var s in musicSources)
                if (s != null) music.Add(s);
    }

    void Start()
    {
        Apply();
        RefreshButtons();
    }

    void Update()
    {
        // Sonradan oluşan sesleri de yakalamak için sık sık kontrol et (menüde/pause'da da çalışır)
        rescanTimer -= Time.unscaledDeltaTime;
        if (rescanTimer <= 0f)
        {
            rescanTimer = 0.1f;
            Apply();
        }
    }

    // ----- Butonlara bağlanacak fonksiyonlar -----

    public void ToggleMusic()
    {
        MusicOn = !MusicOn;
        PlayerPrefs.SetInt(MusicKey, MusicOn ? 1 : 0);
        PlayerPrefs.Save();
        Apply();
        RefreshButtons();
    }

    public void ToggleSfx()
    {
        SfxOn = !SfxOn;
        PlayerPrefs.SetInt(SfxKey, SfxOn ? 1 : 0);
        PlayerPrefs.Save();
        Apply();
        RefreshButtons();
    }

    // ----- Yardımcılar -----

    void Apply()
    {
        var all = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var src in all)
        {
            bool isMusic = music.Contains(src);
            if (!isMusic && detectMusicByName && !string.IsNullOrEmpty(musicNameKeyword) &&
                src.gameObject.name.IndexOf(musicNameKeyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                music.Add(src);
                isMusic = true;
            }
            if (isMusic && !debugMusicFound.Contains(src.gameObject.name))
                debugMusicFound.Add(src.gameObject.name);

            bool shouldMute = isMusic ? !MusicOn : !SfxOn;
            if (src.mute != shouldMute) src.mute = shouldMute;
        }
    }

    void RefreshButtons()
    {
        if (musicLabels != null)
            foreach (var t in musicLabels) if (t != null) t.text = MusicOn ? musicOnText : musicOffText;
        if (sfxLabels != null)
            foreach (var t in sfxLabels) if (t != null) t.text = SfxOn ? sfxOnText : sfxOffText;

        if (musicIcons != null && musicOnSprite != null && musicOffSprite != null)
            foreach (var i in musicIcons) if (i != null) i.sprite = MusicOn ? musicOnSprite : musicOffSprite;
        if (sfxIcons != null && sfxOnSprite != null && sfxOffSprite != null)
            foreach (var i in sfxIcons) if (i != null) i.sprite = SfxOn ? sfxOnSprite : sfxOffSprite;
    }
}