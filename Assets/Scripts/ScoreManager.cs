using TMPro;
using UnityEngine;

// Hayatta kalma süresini ve toplanan yaprak sayısını tutar.
// Oyun sırasında skor panelinde gösterir, oyun bitince Game Over ekranına
// güncel skoru ve en iyi skoru yazar.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    const string BestScoreKey = "BestScore";

    [Header("Oyun sırasındaki skor paneli")]
    [Tooltip("Sadece oyun oynanırken görünür; menüler açıkken gizlenir")]
    [SerializeField] GameObject scoreboardPanel;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text leafText;

    [Header("Game Over ekranındaki yazılar")]
    [SerializeField] TMP_Text currentScoreText;
    [SerializeField] TMP_Text bestScoreText;

    [Header("Puanlama (Score = süre puanı + yaprak puanı)")]
    [SerializeField] int pointsPerSecond = 1;
    [SerializeField] int pointsPerLeaf = 5;

    float survivalTime;
    int leaves;

    public int Score => Mathf.FloorToInt(survivalTime) * pointsPerSecond + leaves * pointsPerLeaf;
    public int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()  => GameMenu.OnGameOver += HandleGameOver;
    void OnDisable() => GameMenu.OnGameOver -= HandleGameOver;

    void Start()
    {
        survivalTime = 0f;
        leaves = 0;
        UpdateScoreboard();
    }

    void Update()
    {
        // Skor paneli sadece oyun akarken görünsün
        if (scoreboardPanel != null && scoreboardPanel.activeSelf != GameMenu.IsPlaying)
            scoreboardPanel.SetActive(GameMenu.IsPlaying);

        if (!GameMenu.IsPlaying) return;

        // Time.deltaTime menüde ve pause'da 0 olduğu için süre o sırada saymaz
        survivalTime += Time.deltaTime;
        UpdateScoreboard();
    }

    // Drenaj temizlenince çağrılır: ScoreManager.Instance.AddLeaves(adet);
    public void AddLeaves(int amount)
    {
        if (!GameMenu.IsPlaying || amount <= 0) return;
        leaves += amount;
        UpdateScoreboard();
    }

    void HandleGameOver()
    {
        int final = Score;
        int best = BestScore;

        if (final > best)
        {
            best = final;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        if (currentScoreText != null) currentScoreText.text = "Score: " + final;
        if (bestScoreText != null)    bestScoreText.text    = "Best: " + best;
    }

    void UpdateScoreboard()
    {
        int seconds = Mathf.FloorToInt(survivalTime);
        if (timeText != null) timeText.text = $"Time: {seconds / 60:00}:{seconds % 60:00}";
        if (leafText != null) leafText.text = "Leaves: " + leaves;
    }
}