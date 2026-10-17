using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class WaterTrail : MonoBehaviour
{
    [Tooltip("Su küpü (RainManager'daki Water objesi)")]
    [SerializeField] Transform water;

    [Tooltip("Karakterin kendisi. Boş bırakılırsa bu objenin bir üstündeki obje kullanılır.")]
    [SerializeField] Transform character;

    [Header("Görünüm")]
    [Tooltip("Yürünen her birim mesafede kaç köpükçük çıksın")]
    [SerializeField] float bubblesPerUnit = 6f;
    [Tooltip("Köpükçüklerin saydamlığı (0 = görünmez, 1 = tam beyaz)")]
    [SerializeField, Range(0f, 1f)] float opacity = 0.5f;
    [Tooltip("Köpükçük boyutu (dünya birimi)")]
    [SerializeField] float bubbleSize = 0.07f;

    [Header("Ayar")]
    [Tooltip("Su, ayakların en az bu kadar üstündeyse iz çıkar")]
    [SerializeField] float minWaterDepth = 0.05f;
    [Tooltip("Köpükçükler yüzeyin ne kadar üstünde dursun")]
    [SerializeField] float surfaceOffset = 0.02f;

    ParticleSystem ps;

    void Awake()
    {
        if (character == null) character = transform.parent;
        ps = GetComponent<ParticleSystem>();
        SetupParticles();
    }

    void LateUpdate()
    {
        if (water == null || character == null) return;

        // Su küpünün üst yüzeyinin yüksekliği
        float waterTop = water.position.y + water.lossyScale.y * 0.5f;
        bool inWater = waterTop - character.position.y >= minWaterDepth;

        // Partikülleri karakterin altında, su yüzeyinde tut
        Vector3 p = character.position;
        transform.position = new Vector3(p.x, waterTop + surfaceOffset, p.z);

        var emission = ps.emission;
        emission.enabled = inWater && GameMenu.IsPlaying;
    }

    void SetupParticles()
    {
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World; // iz geride kalsın
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.9f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(bubbleSize * 0.6f, bubbleSize);
        main.startColor = new Color(1f, 1f, 1f, opacity);
        main.gravityModifier = 0f;
        main.maxParticles = 60;

        var emission = ps.emission;
        emission.rateOverTime = 0f;                 // durunca köpük çıkmasın
        emission.rateOverDistance = bubblesPerUnit; // sadece yürürken

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.25f;
        shape.rotation = new Vector3(90f, 0f, 0f);  // daireyi yere paralel yatır

        var col = ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        col.color = grad;

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.3f));

        // Yumuşak yuvarlak yerine keskin kare (pixel) görünüm
        var rend = GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        if (rend.sharedMaterial != null)
        {
            var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point };
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();

            var mat = rend.material;          // sadece bu objenin kopyası
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
            mat.renderQueue = 3100;           // suyun üstünde çizilsin
        }
    }
}