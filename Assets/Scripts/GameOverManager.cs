using UnityEngine;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign the Game Over panel in the inspector
    public float delayBeforeShowing = 1f; // Delay before showing Game Over screen
    public AudioClip gameOverSound; // Sound to play when Game Over
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameOverPanel.SetActive(false); // Ensure it's inactive at the start
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverWithDelay());
    }

    private IEnumerator ShowGameOverWithDelay()
    {
        // Freeze the game
        Time.timeScale = 0;

        yield return new WaitForSeconds(delayBeforeShowing); // Wait for the specified delay

        if (audioSource && gameOverSound)
        {
            audioSource.PlayOneShot(gameOverSound); // Play sound
        }

        CanvasGroup canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameOverPanel.AddComponent<CanvasGroup>(); // Add if not present
        }

        canvasGroup.alpha = 0; // Start transparent
        gameOverPanel.SetActive(true); // Make sure it's active

        // Fade in
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime; // Adjust fade speed as needed
            yield return null;
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Wait for user input to restart or quit
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R)) // Restart
            {
                RestartGame();
                yield break; // Exit coroutine
            }
            else if (Input.GetKeyDown(KeyCode.Q)) // Quit
            {
                QuitGame();
                yield break; // Exit coroutine
            }
            yield return null;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume time
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // Restart the current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1; // Resume time
        Application.Quit(); // Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
#endif
    }
}
