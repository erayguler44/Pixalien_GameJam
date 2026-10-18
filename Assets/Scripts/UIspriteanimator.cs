using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI Image üzerinde kare kare sprite animasyonu oynatır (örn. karakterin idle animasyonu).
// Hazır bir animasyon klibini (örn. "front idle") Source Clip kutusuna sürükleyince
// kareleri ve hızı otomatik doldurulur. Oyun donukken (menüde) de çalışır.
[RequireComponent(typeof(Image))]
public class UISpriteAnimator : MonoBehaviour
{
    [Tooltip("Mevcut animasyon klibi. Buraya sürükleyince aşağıdaki kareler ve hız otomatik dolar.")]
    [SerializeField] AnimationClip sourceClip;

    [Tooltip("Animasyon kareleri, sırasıyla")]
    [SerializeField] Sprite[] frames;

    [Tooltip("Saniyede kaç kare")]
    [SerializeField] float fps = 6f;

    Image img;
    float time;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    void OnEnable()
    {
        time = 0f;
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        time += Time.unscaledDeltaTime;
        int index = Mathf.FloorToInt(time * fps) % frames.Length;
        img.sprite = frames[index];
    }

#if UNITY_EDITOR
    // Editörde klip değiştirilince karelerini ve hızını klipten okur
    void OnValidate()
    {
        if (sourceClip == null) return;

        foreach (var binding in UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(sourceClip))
        {
            if (binding.propertyName != "m_Sprite") continue;

            var keys = UnityEditor.AnimationUtility.GetObjectReferenceCurve(sourceClip, binding);
            if (keys == null || keys.Length == 0) continue;

            var list = new List<Sprite>();
            foreach (var k in keys)
                if (k.value is Sprite s) list.Add(s);

            // Döngü için sona eklenmiş tekrar karesini at
            if (list.Count > 1 && list[list.Count - 1] == list[0]) list.RemoveAt(list.Count - 1);

            frames = list.ToArray();

            if (keys.Length > 1)
            {
                float step = keys[1].time - keys[0].time;
                if (step > 0f) fps = 1f / step;
            }

            // Editörde de ilk kareyi göster
            var image = GetComponent<Image>();
            if (image != null && frames.Length > 0) image.sprite = frames[0];
            break;
        }
    }
#endif
}