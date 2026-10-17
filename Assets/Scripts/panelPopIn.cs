using System.Collections;
using UnityEngine;

// Panele eklenir. Panel her açıldığında (SetActive(true)) içerik
// küçükten büyüyüp hafifçe taşarak yerine oturur ve aynı anda belirir.
[RequireComponent(typeof(CanvasGroup))]
public class PanelPopIn : MonoBehaviour
{
    [Tooltip("Büyüyecek kısım (örn. butonların olduğu obje). Boş bırakılırsa tüm panel büyür.")]
    [SerializeField] RectTransform content;

    [Tooltip("Animasyon süresi (saniye)")]
    [SerializeField] float duration = 0.35f;

    [Tooltip("Başlangıç boyutu (0.6 = %60)")]
    [SerializeField] float startScale = 0.6f;

    [Tooltip("Sıçrama miktarı. 0 = taşma yok, büyüdükçe daha çok 'zıplar'")]
    [SerializeField] float overshoot = 1.7f;

    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (content == null) content = (RectTransform)transform;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        group.alpha = 0f;
        content.localScale = Vector3.one * startScale;

        float t = 0f;
        while (t < duration)
        {
            // unscaledDeltaTime: oyun durmuşken (Time.timeScale = 0) de çalışsın diye
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);

            group.alpha = Mathf.Clamp01(p * 2f); // Görünürlük boyuttan biraz daha hızlı tamamlanır
            content.localScale = Vector3.one * Mathf.LerpUnclamped(startScale, 1f, EaseOutBack(p));

            yield return null;
        }

        group.alpha = 1f;
        content.localScale = Vector3.one;
    }

    // Hedefi biraz aşıp geri dönen yumuşak geçiş
    float EaseOutBack(float x)
    {
        float c1 = overshoot;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
    }
}