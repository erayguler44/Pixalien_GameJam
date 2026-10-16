using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Diğer scriptler oyunun akıp akmadığını buradan kontrol eder
    public static bool IsPlaying { get; private set; }

    [SerializeField] GameObject mainMenuPanel;  // Play / Quit
    [SerializeField] GameObject pauseMenuPanel; // Resume / Main Menu
    [SerializeField] GameObject pauseBlur;      // Durunca açılan bulanıklık Volume'ü (opsiyonel)

    enum State { MainMenu, Playing, Paused }
    State state;

    void Start()
    {
        SetState(State.MainMenu);
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (state == State.Playing) SetState(State.Paused);
        else if (state == State.Paused) SetState(State.Playing);
        // Ana menüdeyken ESC hiçbir şey yapmaz
    }

    // ----- Butonlar -----

    public void Play()   => SetState(State.Playing);
    public void Resume() => SetState(State.Playing);

    public void MainMenu()
    {
        // Sahneyi baştan yükle: her şey sıfırlanır, Start() ana menüyü açar
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ----- Durum yönetimi -----

    void SetState(State newState)
    {
        state = newState;
        IsPlaying = state == State.Playing;

        mainMenuPanel.SetActive(state == State.MainMenu);
        pauseMenuPanel.SetActive(state == State.Paused);
        if (pauseBlur != null) pauseBlur.SetActive(state == State.Paused);

        // Oyun sadece Playing durumunda akar
        Time.timeScale = IsPlaying ? 1f : 0f;
    }
}
