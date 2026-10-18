using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI içinde (menü panelinde) pixel yağmur yağdırır.
// Boş bir objeye eklenir; o objenin kapladığı alana yağar.
// Oyun donukken (Time.timeScale = 0) de çalışır.
[RequireComponent(typeof(RectTransform))]
public class UIRain : MonoBehaviour
{
    [Header("Görseller")]
    [Tooltip("Damla çizimi")]
    [SerializeField] Sprite dropSprite;
    [SerializeField] Vector2 dropSize = new Vector2(12, 24);
    [Tooltip("Yere düşünce oynayacak sıçrama/halka kareleri (opsiyonel)")]
    [SerializeField] Sprite[] splashFrames;
    [SerializeField] Vector2 splashSize = new Vector2(48, 48);
    [SerializeField] float splashFps = 12f;
    [SerializeField] Color color = Color.white;

    [Header("Yağış")]
    [SerializeField] float dropsPerSecond = 30f;
    [Tooltip("Düşme hızı aralığı (piksel/saniye)")]
    [SerializeField] Vector2 fallSpeed = new Vector2(900, 1300);
    [Tooltip("Yatay rüzgar (piksel/saniye). Eksi sola, artı sağa.")]
    [SerializeField] float wind = 0f;
    [Tooltip("Damlaların yere düştüğü yükseklik aralığı (0 = alt kenar, 1 = üst kenar)")]
    [SerializeField, Range(0, 1)] float groundMin = 0.05f;
    [SerializeField, Range(0, 1)] float groundMax = 0.3f;
    [Tooltip("Hareket bu kadar piksellik adımlarla olur; çizimlerinizin piksel boyutuyla aynı yapın")]
    [SerializeField] float pixelSize = 4f;
    [Tooltip("Panel açıldığında ekran boş başlamasın diye kaç saniye önceden yağdırılsın")]
    [SerializeField] float prewarmSeconds = 1.5f;
    [SerializeField] int maxDrops = 300;

    class Drop
    {
        public RectTransform rt;
        public Image img;
        public Vector2 pos;
        public Vector2 vel;
        public float groundY;
        public bool splashing;
        public float splashTime;
    }

    readonly List<Drop> active = new List<Drop>();
    readonly Stack<Drop> pool = new Stack<Drop>();
    RectTransform area;
    float spawnTimer;

    void Awake()
    {
        area = (RectTransform)transform;
    }

    void OnEnable()
    {
        for (float t = 0f; t < prewarmSeconds; t += 1f / 30f)
            Step(1f / 30f);
    }

    void OnDisable()
    {
        foreach (var d in active) Recycle(d, false);
        active.Clear();
    }

    void Update()
    {
        Step(Time.unscaledDeltaTime);
    }

    void Step(float dt)
    {
        if (dropSprite == null) return;
        Vector2 size = area.rect.size;

        // Yeni damlalar
        spawnTimer += dt * dropsPerSecond;
        while (spawnTimer >= 1f)
        {
            spawnTimer -= 1f;
            if (active.Count < maxDrops) Spawn(size);
        }

        // Hareket
        for (int i = active.Count - 1; i >= 0; i--)
        {
            Drop d = active[i];

            if (d.splashing)
            {
                d.splashTime += dt;
                int frame = Mathf.FloorToInt(d.splashTime * splashFps);
                if (frame >= splashFrames.Length)
                {
                    Recycle(d, true);
                    active.RemoveAt(i);
                }
                else d.img.sprite = splashFrames[frame];
                continue;
            }

            d.pos += d.vel * dt;

            if (d.pos.y <= d.groundY)
            {
                if (splashFrames != null && splashFrames.Length > 0)
                {
                    d.splashing = true;
                    d.splashTime = 0f;
                    d.pos.y = d.groundY;
                    d.img.sprite = splashFrames[0];
                    d.rt.sizeDelta = splashSize;
                }
                else
                {
                    Recycle(d, true);
                    active.RemoveAt(i);
                    continue;
                }
            }

            Place(d);
        }
    }

    void Spawn(Vector2 size)
    {
        Drop d = pool.Count > 0 ? pool.Pop() : Create();
        d.rt.gameObject.SetActive(true);

        // Rüzgar varsa damlalar ekrana yandan da girebilsin diye başlangıç alanını genişlet
        float speed = Random.Range(fallSpeed.x, fallSpeed.y);
        float travelTime = size.y / speed;
        float extra = -wind * travelTime;

        d.pos = new Vector2(Random.Range(Mathf.Min(0, extra), size.x + Mathf.Max(0, extra)), size.y + dropSize.y);
        d.vel = new Vector2(wind, -speed);
        d.groundY = Random.Range(groundMin, groundMax) * size.y;
        d.splashing = false;
        d.img.sprite = dropSprite;
        d.img.color = color;
        d.rt.sizeDelta = dropSize;

        Place(d);
        active.Add(d);
    }

    void Place(Drop d)
    {
        Vector2 p = d.pos;
        if (pixelSize > 0f)
        {
            p.x = Mathf.Round(p.x / pixelSize) * pixelSize;
            p.y = Mathf.Round(p.y / pixelSize) * pixelSize;
        }
        d.rt.anchoredPosition = p;
    }

    Drop Create()
    {
        var go = new GameObject("Drop", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        var d = new Drop
        {
            rt = (RectTransform)go.transform,
            img = go.GetComponent<Image>()
        };
        d.rt.anchorMin = d.rt.anchorMax = Vector2.zero; // sol alt köşeye göre konum
        d.rt.pivot = new Vector2(0.5f, 0f);              // alt-orta: halka yere oturur
        d.img.raycastTarget = false;                     // butonlara tıklamayı engellemesin
        return d;
    }

    void Recycle(Drop d, bool pushToPool)
    {
        d.rt.gameObject.SetActive(false);
        if (pushToPool) pool.Push(d);
        else pool.Push(d);
    }
}