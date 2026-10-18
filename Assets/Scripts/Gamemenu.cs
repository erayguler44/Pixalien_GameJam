using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Diğer scriptler buradan erişir: GameMenu.Instance.GameOver();
    public static GameMenu Instance { get; private set; }

    // Diğer scriptler oyunun akıp akmadığını buradan kontrol eder
    public static bool IsPlaying { get; private set; }

    // Giriş ekranı açık mı (MainMenuAmbience gibi scriptler kullanır)
    public static bool IsInMainMenu { get; private set; }

    // Play Again'e basılınca sahne yeniden yüklenir; bu bayrak ana menüyü atlatır
    static bool skipMainMenu = false;

    [SerializeField] GameObject mainMenuPanel;  // Play / Quit
    [SerializeField] GameObject pauseMenuPanel; // Resume / Main Menu
    [SerializeField] GameObject gameOverPanel;  // Play Again / Main Menu
    [SerializeField] GameObject tutorialPanel;  // Nasıl oynanır + Back butonu
    [SerializeField] GameObject pauseBlur;      // Bulanıklık Volume'ü (opsiyonel)

    enum State { MainMenu, Playing, Paused, GameOver, Tutorial }
    State state;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (skipMainMenu)
        {
            skipMainMenu = false;
            SetState(State.Playing);
        }
        else
        {
            SetState(State.MainMenu);
        }
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (state == State.Playing) SetState(State.Paused);
        else if (state == State.Paused) SetState(State.Playing);
        else if (state == State.Tutorial) SetState(State.MainMenu);
        // Ana menüde ve oyun bitti ekranında ESC hiçbir şey yapmaz
    }

    // ----- Oyuncu kaybedince başka scriptten çağrılır -----

    // Oyun bittiği an haber almak isteyen scriptler buna abone olur (örn. ScoreManager)
    public static event System.Action OnGameOver;

    public void GameOver()
    {
        if (state != State.Playing) return;

        OnGameOver?.Invoke();   // Önce skorlar hesaplansın
        SetState(State.GameOver);
    }

    // ----- Butonlar -----

    public void Play()   => SetState(State.Playing);
    public void Resume() => SetState(State.Playing);
    public void OpenTutorial()  => SetState(State.Tutorial);
    public void CloseTutorial() => SetState(State.MainMenu);

    public void PlayAgain()
    {
        skipMainMenu = true;   // Yeniden yüklenince doğrudan oyun başlasın
        ReloadScene();
    }

    public void MainMenu()
    {
        skipMainMenu = false;  // Yeniden yüklenince ana menü açılsın
        ReloadScene();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ----- Yardımcılar -----

    void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetState(State newState)
    {
        state = newState;
        IsPlaying = state == State.Playing;
        IsInMainMenu = state == State.MainMenu;

        mainMenuPanel.SetActive(state == State.MainMenu);
        pauseMenuPanel.SetActive(state == State.Paused);
        gameOverPanel.SetActive(state == State.GameOver);
        if (tutorialPanel != null) tutorialPanel.SetActive(state == State.Tutorial);

        if (pauseBlur != null)
            pauseBlur.SetActive(state == State.Paused || state == State.GameOver);

        // Oyun sadece Playing durumunda akar
        Time.timeScale = IsPlaying ? 1f : 0f;
    }
}