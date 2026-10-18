using UnityEngine;

// Herhangi bir Particle System'e eklenir. Yuvarlak/yumuşak partikülleri
// keskin, pikselli karelere çevirir. Başka hiçbir ayara dokunmaz
// (hız, renk, sayı, boyut aynen kalır). İçindeki alt partikül sistemlerine de uygulanır.
public class PixelParticles : MonoBehaviour
{
    [Tooltip("Açıksa bu objenin altındaki tüm partikül sistemleri de kare olur (sub emitter'lar dahil)")]
    [SerializeField] bool includeChildren = true;

    [Tooltip("Saydamlık. 1 = hiç değiştirme, 0.5 = yarı saydam")]
    [SerializeField, Range(0f, 1f)] float opacity = 1f;

    static Texture2D squareTex;

    void Awake()
    {
        if (squareTex == null)
        {
            squareTex = new Texture2D(1, 1) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            squareTex.SetPixel(0, 0, Color.white);
            squareTex.Apply();
        }

        var renderers = includeChildren
            ? GetComponentsInChildren<ParticleSystemRenderer>(true)
            : GetComponents<ParticleSystemRenderer>();

        foreach (var r in renderers)
        {
            if (r.sharedMaterial == null) continue;

            // r.material: sadece bu efektin kopyası değişir, ortak materyal dosyası bozulmaz
            var mat = r.material;
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", squareTex);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", squareTex);

            if (opacity < 1f) MakeTransparent(mat);
        }
    }

    void MakeTransparent(Material mat)
    {
        // Materyali saydam moda al (URP Particles / Unlit ve Lit materyalleri için)
        if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
        if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 0f);
        if (mat.HasProperty("_AlphaClip")) mat.SetFloat("_AlphaClip", 0f);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.SetOverrideTag("RenderType", "Transparent");

        if (mat.HasProperty("_SrcBlend")) mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (mat.HasProperty("_DstBlend")) mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        if (mat.HasProperty("_SrcBlendAlpha")) mat.SetFloat("_SrcBlendAlpha", (float)UnityEngine.Rendering.BlendMode.One);
        if (mat.HasProperty("_DstBlendAlpha")) mat.SetFloat("_DstBlendAlpha", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 0f);
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Materyal rengini saydamlıkla çarp (partikülün kendi renkleri korunur)
        string colorProp = mat.HasProperty("_BaseColor") ? "_BaseColor" : (mat.HasProperty("_Color") ? "_Color" : null);
        if (colorProp != null)
        {
            Color c = mat.GetColor(colorProp);
            c.a *= opacity;
            mat.SetColor(colorProp, c);
        }
    }
}