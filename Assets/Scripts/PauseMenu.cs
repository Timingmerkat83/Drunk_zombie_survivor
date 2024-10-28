using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Référence au menu de pause
    private static bool isPaused = false; // État de pause



    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        // Vérifiez si la touche Esc est pressée
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }

    public void Play()
    {
        pauseMenuUI.SetActive(false); // Désactive le menu
        Time.timeScale = 1f; // Reprend le jeu
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
         Debug.Log("Resumed Game");
    }

    void Stop()
    {
        pauseMenuUI.SetActive(true); // Affiche le menu
        Time.timeScale = 0f; // Met le jeu en pause
        isPaused = true;
         Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
{
    // Optionally, you can reload the current scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    Debug.Log("Game restarted");
}

    public void QuitGame()
    {
        // Quitte le jeu
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Pour l'éditeur
        #endif
        Debug.Log("Game quitted");
    }
}
