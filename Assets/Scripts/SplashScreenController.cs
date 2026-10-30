using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("Name of the scene to load after splash video finishes")]
    [SerializeField] private string nextSceneName = "MainScene";

    [Header("Options")]
    [Tooltip("Allow the player to skip the video by pressing any key or clicking")]
    [SerializeField] private bool allowSkip = true;

    private VideoPlayer videoPlayer;
    private bool isTransitioning = false;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // Ensure the video does not loop
        videoPlayer.isLooping = false;

        // Subscribe to the video finished event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void Update()
    {
        // Optional: Skip playback on user input
        if (allowSkip && !isTransitioning)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                TransitionToNextScene();
            }
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        TransitionToNextScene();
    }

    private void TransitionToNextScene()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        // Unsubscribe to avoid memory leaks
        videoPlayer.loopPointReached -= OnVideoEnd;

        SceneManager.LoadScene(nextSceneName);
    }
}