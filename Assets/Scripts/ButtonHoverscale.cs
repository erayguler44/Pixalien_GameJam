using UnityEngine;
using UnityEngine.EventSystems;

// Butona eklenir. Fare üstüne gelince buton yumuşakça büyür, çıkınca eski haline döner.
// Button'un Color Tint / Sprite Swap ayarlarına dokunmaz, onlarla birlikte çalışır.
public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Fare üstündeyken ne kadar büyüsün (1.1 = %10 büyük)")]
    [SerializeField] float hoverScale = 1.1f;

    [Tooltip("Büyüme/küçülme hızı")]
    [SerializeField] float speed = 12f;

    Vector3 originalScale;
    Vector3 targetScale;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void OnDisable()
    {
        // Panel kapanırken buton büyük kalmışsa, tekrar açıldığında normal boyutta görünsün
        transform.localScale = originalScale;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    void Update()
    {
        // unscaledDeltaTime: oyun durmuşken (Time.timeScale = 0) menülerde de çalışsın diye
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Time.unscaledDeltaTime);
    }
}