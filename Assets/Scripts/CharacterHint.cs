using UnityEngine;

// Karaktere eklenir. Oyun başlayınca karakterin kafasının üstünde
// fare animasyonu + yazı gösterir, belirli bir süre sonra sönerek kaybolur.
// Süre sadece oyun oynanırken sayar (menüde / pause'da beklemez).
public class HeadHint : MonoBehaviour
{
    [Header("Görseller")]
    [Tooltip("Fare animasyonunun kareleri (RmbClickSheet_0..3)")]
    [SerializeField] Sprite[] frames;
    [Tooltip("Saniyede kaç kare")]
    [SerializeField] float fps = 10f;
    [Tooltip("Yanındaki yazı (ClickRmbToWalk)")]
    [SerializeField] Sprite textSprite;

    [Header("Boyut ve konum")]
    [Tooltip("Farenin dünyadaki yüksekliği (birim). Yazı aynı piksel ölçeğinde otomatik boyutlanır.")]
    [SerializeField] float iconSize = 0.6f;
    [Tooltip("Fare ile yazı arasındaki boşluk (çizim pikseli cinsinden, eksi değer yaklaştırır)")]
    [SerializeField] float spacingPixels = -6f;
    [Tooltip("Karakterin kafasından ne kadar yukarıda dursun")]
    [SerializeField] float gapAboveHead = 0.15f;

    [Header("Süre")]
    [SerializeField] float showDuration = 5f;
    [SerializeField] float fadeDuration = 0.4f;
    [SerializeField] int sortingOrder = 60;

    Transform root;
    SpriteRenderer iconSr, textSr;
    Camera cam;
    float headOffset;      // karakterin pozisyonundan kafasının üstüne olan yükseklik
    float shownTime;
    float animTime;

    void Start()
    {
        if (frames == null || frames.Length == 0) { enabled = false; return; }
        cam = Camera.main;

        // Kafanın yüksekliğini bir kere ölç (karakter sprite'ları; partiküller hariç)
        bool found = false;
        Bounds b = new Bounds(transform.position, Vector3.zero);
        foreach (var r in GetComponentsInChildren<SpriteRenderer>())
        {
            if (!r.enabled) continue;
            if (!found) { b = r.bounds; found = true; } else b.Encapsulate(r.bounds);
        }
        headOffset = (found ? b.max.y : transform.position.y + 1f) - transform.position.y;

        // Karaktere bağlamıyoruz: karakter yön değiştirirken ölçeği ters çevrilirse yazı aynalanmasın
        root = new GameObject("HeadHint").transform;

        iconSr = CreateSprite("Mouse", frames[0]);
        if (textSprite != null) textSr = CreateSprite("Text", textSprite);

        Layout();
        SetAlpha(0f);
    }

    SpriteRenderer CreateSprite(string name, Sprite s)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = sortingOrder;
        return sr;
    }

    void Layout()
    {
        // Fare çiziminin bir pikselinin dünyadaki boyu; yazı da aynı ölçekte
        float iconPx = frames[0].rect.height;
        float unitPerPx = iconSize / iconPx;

        float iconW = frames[0].rect.width * unitPerPx;
        float textW = textSprite != null ? textSprite.rect.width * unitPerPx : 0f;
        float gap = textSprite != null ? spacingPixels * unitPerPx : 0f;
        float total = iconW + gap + textW;
        float left = -total * 0.5f;

        SetWorldSize(iconSr, unitPerPx);
        iconSr.transform.localPosition = new Vector3(left + iconW * 0.5f, 0f, 0f);

        if (textSr != null)
        {
            SetWorldSize(textSr, unitPerPx);
            textSr.transform.localPosition = new Vector3(left + iconW + gap + textW * 0.5f, 0f, 0f);
        }
    }

    static void SetWorldSize(SpriteRenderer sr, float unitPerPx)
    {
        // Sprite'ın kendi Pixels Per Unit değerinden bağımsız olarak piksel başına unitPerPx
        float s = unitPerPx * sr.sprite.pixelsPerUnit;
        sr.transform.localScale = new Vector3(s, s, 1f);
    }

    void LateUpdate()
    {
        if (root == null) return;

        // Süre sadece oyun oynanırken sayar
        if (GameMenu.IsPlaying) shownTime += Time.deltaTime;

        float remaining = showDuration - shownTime;
        if (remaining <= 0f)
        {
            Destroy(root.gameObject);
            enabled = false;
            return;
        }

        // Menüde görünmesin; oyunda belirsin, sonunda sönsün
        float alpha = GameMenu.IsPlaying || shownTime > 0f ? 1f : 0f;
        if (fadeDuration > 0f) alpha *= Mathf.Clamp01(remaining / fadeDuration);
        SetAlpha(alpha);

        // Animasyon
        animTime += Time.deltaTime;
        iconSr.sprite = frames[Mathf.FloorToInt(animTime * fps) % frames.Length];

        // Karakteri takip et ve kameraya dön
        root.position = transform.position + Vector3.up * (headOffset + gapAboveHead + iconSize * 0.5f);
        if (cam != null) root.rotation = cam.transform.rotation;
    }

    void SetAlpha(float a)
    {
        var c = Color.white; c.a = a;
        iconSr.color = c;
        if (textSr != null) textSr.color = c;
    }

    void OnDestroy()
    {
        if (root != null) Destroy(root.gameObject);
    }
}