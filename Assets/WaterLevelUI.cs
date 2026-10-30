using UnityEngine;
using UnityEngine.UI;

// Su seviyesi barı + sel yaklaşınca kalp atışı gibi yanıp sönen kırmızı ekran.
// Su yüksekliğini RainManager'dan okur, oyun mantığına dokunmaz.
public class WaterLevelUI : MonoBehaviour
{
    [Header("Kaynak")]
    [SerializeField] RainManager rainManager;

    [Header("Bar")]
    [Tooltip("Barın dolan kısmı. Image Type 'Filled' ise fillAmount, değilse dikey ölçek kullanılır.")]
    [SerializeField] Image fillImage;
    [Tooltip("Seviyeye göre bar rengi (soldan sağa: boş -> dolu)")]
    [SerializeField] Gradient fillColor;
    [Tooltip("Barın hedef seviyeye ne kadar hızlı yetiştiği")]
    [SerializeField] float smoothSpeed = 6f;

    [Header("Tehlike efekti (kırmızı ekran)")]
    [SerializeField] Image dangerOverlay;
    [Tooltip("Bu seviyeden sonra kırmızı efekt başlar (0-1)")]
    [SerializeField, Range(0f, 1f)] float dangerStart = 0.7f;
    [Tooltip("En yüksek kırmızılık")]
    [SerializeField, Range(0f, 1f)] float maxAlpha = 0.55f;
    [Tooltip("Tehlike başlarken ve sele en yakınken kalp atış hızı (atış/dakika)")]
    [SerializeField] float minBpm = 60f;
    [SerializeField] float maxBpm = 160f;

    float shownLevel;
    float beatPhase;
    float overlayAlpha;

    void Reset()
    {
        // Varsayılan bar rengi: mavi -> sarı -> kırmızı
        fillColor = new Gradient();
        fillColor.SetKeys(
            new[] {
                new GradientColorKey(new Color(0.25f, 0.55f, 1f), 0f),
                new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0.6f),
                new GradientColorKey(new Color(1f, 0.2f, 0.2f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
    }

    void Start()
    {
        if (rainManager == null) rainManager = FindFirstObjectByType<RainManager>();
        SetOverlay(0f);
        ApplyBar(0f);
    }

    void Update()
    {
        // Bar: gerçek seviyeye yumuşakça yaklaş
        float target = CurrentLevel();
        shownLevel = Mathf.Lerp(shownLevel, target, 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime));
        ApplyBar(shownLevel);

        // Tehlike efekti: sadece oyun oynanırken ve su tehlike seviyesini geçtiyse
        float danger = Mathf.InverseLerp(dangerStart, 1f, shownLevel);
        float targetAlpha = 0f;

        if (GameMenu.IsPlaying && danger > 0f)
        {
            float bpm = Mathf.Lerp(minBpm, maxBpm, danger);
            beatPhase = (beatPhase + Time.deltaTime * bpm / 60f) % 1f;

            // "Güm-güm ... güm-güm": iki atış, sonra kısa sessizlik
            float pulse = Mathf.Max(Bump(beatPhase, 0.08f, 0.08f), 0.6f * Bump(beatPhase, 0.30f, 0.08f));
            targetAlpha = maxAlpha * danger * (0.25f + 0.75f * pulse);
            overlayAlpha = targetAlpha;   // atışlar keskin olsun
        }
        else
        {
            // Oyun durunca / tehlike geçince yavaşça sön
            overlayAlpha = Mathf.MoveTowards(overlayAlpha, 0f, Time.unscaledDeltaTime * 2f);
        }

        SetOverlay(overlayAlpha);
    }

    float CurrentLevel()
    {
        if (rainManager == null || rainManager.waterCube == null) return 0f;
        return Mathf.InverseLerp(rainManager.minWaterY, rainManager.maxWaterY, rainManager.waterCube.position.y);
    }

    void ApplyBar(float level)
    {
        if (fillImage == null) return;

        if (fillImage.type == Image.Type.Filled)
            fillImage.fillAmount = level;
        else
            fillImage.rectTransform.localScale = new Vector3(1f, level, 1f); // pivot Y = 0 olmalı

        if (fillColor != null) fillImage.color = fillColor.Evaluate(level);
    }

    void SetOverlay(float a)
    {
        if (dangerOverlay == null) return;
        var c = dangerOverlay.color;
        c.a = a;
        dangerOverlay.color = c;
    }

    // center etrafında genişliği width olan üçgen tepe (0-1)
    static float Bump(float t, float center, float width)
    {
        return Mathf.Clamp01(1f - Mathf.Abs(t - center) / width);
    }
}