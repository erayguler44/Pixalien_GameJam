using UnityEngine;

// Karton kutuya eklenir. Kutunun tam üstünde "sol tıkla" fare animasyonunu
// kendisi oluşturur ve oynatır. Ayrı obje kurmaya gerek yoktur.
// Kutu silinir ya da görüntüsü kapanırsa fare de kaybolur.
public class BoxClickHint : MonoBehaviour
{
    [Tooltip("Fare animasyonunun kareleri, sırasıyla (ClickSpamSheet_0..3)")]
    [SerializeField] Sprite[] frames;

    [Tooltip("Saniyede kaç kare (GIF'teki hız: 10)")]
    [SerializeField] float fps = 10f;

    [Tooltip("Farenin dünyadaki yüksekliği (birim)")]
    [SerializeField] float iconSize = 0.6f;

    [Tooltip("Kutunun tepesinden ne kadar yukarıda dursun")]
    [SerializeField] float gapAboveBox = 0.15f;

    [Tooltip("Oyuncu bu mesafeden yakınsa görünür. 0 = her zaman görünür.")]
    [SerializeField] float showRadius = 0f;

    [Tooltip("Diğer sprite'ların önünde çizilsin diye")]
    [SerializeField] int sortingOrder = 50;

    Transform hint;
    SpriteRenderer sr;
    Renderer[] boxRenderers;
    Transform player;
    Camera cam;
    float animTime;

    void Start()
    {
        if (frames == null || frames.Length == 0) { enabled = false; return; }

        // Kutunun kendi görselleri (fareyi eklemeden önce topla)
        boxRenderers = GetComponentsInChildren<Renderer>(true);

        hint = new GameObject("ClickHint").transform;
        hint.SetParent(transform, false);
        sr = hint.gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = frames[0];
        sr.sortingOrder = sortingOrder;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (hint == null) return;

        // Kutu görünmüyorsa (temizlendi / kapatıldı) fareyi de gizle
        bool boxVisible = false;
        Bounds b = new Bounds(transform.position, Vector3.zero);
        foreach (var r in boxRenderers)
        {
            if (r == null || !r.enabled || !r.gameObject.activeInHierarchy) continue;
            if (!boxVisible) { b = r.bounds; boxVisible = true; }
            else b.Encapsulate(r.bounds);
        }

        bool show = boxVisible;
        if (show && showRadius > 0f && player != null)
        {
            Vector3 a = transform.position, c = player.position;
            a.y = c.y = 0f;
            show = Vector3.Distance(a, c) <= showRadius;
        }
        sr.enabled = show;
        if (!show) return;

        // Animasyon (Time.deltaTime: oyun durunca animasyon da durur)
        animTime += Time.deltaTime;
        sr.sprite = frames[Mathf.FloorToInt(animTime * fps) % frames.Length];

        // Kameraya dön
        if (cam != null) hint.rotation = cam.transform.rotation;

        // Boyut: kutunun ölçeğinden bağımsız, dünyada iconSize yüksekliğinde
        float spriteH = sr.sprite.bounds.size.y;
        Vector3 parentScale = transform.lossyScale;
        float s = spriteH > 0f ? iconSize / spriteH : 1f;
        hint.localScale = new Vector3(
            s / Mathf.Max(0.0001f, Mathf.Abs(parentScale.x)),
            s / Mathf.Max(0.0001f, Mathf.Abs(parentScale.y)),
            s / Mathf.Max(0.0001f, Mathf.Abs(parentScale.z)));

        // Konum: kutunun tepesinin ortası + boşluk + farenin yarı yüksekliği
        hint.position = new Vector3(b.center.x, b.max.y + gapAboveBox + iconSize * 0.5f, b.center.z);
    }
}