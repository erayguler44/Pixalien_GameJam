using UnityEngine;

// Kutunun yanındaki "sol tıkla spamla" animasyonuna eklenir.
// Oyuncu yaklaşınca görünür, uzaklaşınca gizlenir; her zaman kameraya döner.
// Kutu yok olunca (child olduğu için) o da kendiliğinden kaybolur.
public class ClickHint : MonoBehaviour
{
    [Tooltip("Oyuncu bu mesafeden yakınsa görünür. 0 = her zaman görünür.")]
    [SerializeField] float showRadius = 3f;

    [Tooltip("Açıksa sprite her zaman kameraya dönük durur")]
    [SerializeField] bool faceCamera = true;

    Transform player;
    Camera cam;
    SpriteRenderer[] renderers;
    bool visible = true;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        cam = Camera.main;
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        SetVisible(showRadius <= 0f);
    }

    void LateUpdate()
    {
        if (faceCamera && cam != null)
            transform.rotation = cam.transform.rotation;

        bool show = showRadius <= 0f;
        if (!show && player != null)
        {
            // Sadece yatay mesafe (yükseklik farkı önemli değil)
            Vector3 a = transform.position, b = player.position;
            a.y = b.y = 0f;
            show = Vector3.Distance(a, b) <= showRadius;
        }

        if (show != visible) SetVisible(show);
    }

    void SetVisible(bool v)
    {
        visible = v;
        // Objeyi kapatmak yerine sadece görüntüyü gizle: animasyon arka planda devam eder
        foreach (var r in renderers) r.enabled = v;
    }
}